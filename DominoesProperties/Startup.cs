using System;
using System.IO;
using System.Reflection;
using System.Text;
using DominoesProperties.Controllers;
using DominoesProperties.Extensions;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using DominoesProperties.Services;
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
            services.AddDbContext<dominoespropertiesContext>(opts => opts.UseMySQL(Configuration.GetConnectionString("DominoProps_String")));
            #endregion
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Redis").GetValue<String>("Host");
                options.InstanceName = Configuration.GetSection("Redis").GetValue<String>("InstanceName");
            });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Domino Properties v1"));
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
            });
        }
    }
}