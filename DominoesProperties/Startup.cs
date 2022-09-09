using System;
using System.IO;
using System.Reflection;
using System.Text;
using DominoesProperties.Controllers;
using DominoesProperties.Extensions;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using DominoesProperties.Scheduled;
using DominoesProperties.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using HangfireBasicAuthenticationFilter;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.Context;
using Newtonsoft.Json;
using NLog;
using Repositories.Repository;
using Repositories.Service;
using StackExchange.Redis;

namespace DominoesProperties
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure jwt authentication
            #region JWT Token
            var appSettingsSection = Configuration.GetSection("app_settings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(30),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["app_settings:Issuer"],
                    ValidAudience = Configuration["app_settings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            #endregion

            services.AddScoped<ICustomerRepository, CustomerService>();
            services.AddScoped<IWalletRepository, WalletService>();
            services.AddScoped<IPropertyRepository, PropertyService>();
            services.AddScoped<IUtilRepository, UtilServices>();
            services.AddScoped<ITransactionRepository, TransactionService>();
            services.AddScoped<IPaystackRepository, PaystackService>();
            services.AddScoped<IInvestmentRepository, InvestmentService>();
            services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsService>();
            services.AddScoped<PaymentController, PaymentController>();
            services.AddScoped<IAdminRepository, AdminService>();
            services.AddScoped<IUploadRepository, UploadService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IDominoJob, DominoJob>();

            services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            services.AddJsonLocalization(opt => opt.ResourcesPath = "Resources");

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dominoes Society", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                var filePath = Path.Combine(string.Concat(Directory.GetCurrentDirectory()), "DominoesProperties.xml");
                c.IncludeXmlComments(filePath);
            });

            #region Connection String
            services.AddDbContext<dominoespropertiesContext>(opts =>
                opts.UseMySQL(Configuration.GetConnectionString("DominoProps_String"))
            );
            #endregion
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            var deltaBackOffMilliseconds = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
            var maxDeltaBackOffMilliseconds = Convert.ToInt32(TimeSpan.FromSeconds(20).TotalMilliseconds);
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { $"{Configuration.GetValue<string>("Redis:Host")}:{Configuration.GetValue<int>("Redis:Port")}" },
                Ssl = Configuration.GetValue<bool>("Redis:Ssl"),
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ReconnectRetryPolicy = new ExponentialRetry(deltaBackOffMilliseconds, maxDeltaBackOffMilliseconds),
                ConnectTimeout = 3000,
                DefaultDatabase = 0,
                AllowAdmin = true,
                Password = Configuration.GetValue<string>("Redis:Password"),
                User = "default"
            };
            var multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.ConfigureNLogService();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            // Add Hangfire services.
            string hangfireConnectionString = Configuration.GetConnectionString("DominoProps_String");
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(
                    new MySqlStorage(
                        hangfireConnectionString,
                        new MySqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(10),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 25000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablesPrefix = "Hangfire",
                        }
                    )
                ));

            // Add the processing server as IHostedService
            services.AddHangfireServer(options => options.WorkerCount = 1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Domino Properties v1"));
            }
            
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                var result = JsonConvert.SerializeObject(new { error = "Some technical error Occurred, please visit after sometime" });
                context.Response.ContentType = "application/json";
                CommonLogic.SendExceptionEmail("Exception Occurred", "Error On Method :  " + MethodBase.GetCurrentMethod().DeclaringType.Name + " and Message : " + exception.Message + "<br> StackTrace : " + exception.StackTrace);
                await context.Response.WriteAsync(result);
            }));

            app.UseStaticFiles();

            app.UseCors("AllowAllHeaders");

            app.UseAuthentication();

            app.ConfigureCustomExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new HangFireAuth() },
                    IsReadOnlyFunc = (DashboardContext context) => true,
                    AppPath = Configuration.GetValue<string>("app_settings:WebEndpoint")
                });
            });
        }
    }
}