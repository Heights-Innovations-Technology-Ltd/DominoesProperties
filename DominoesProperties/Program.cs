using System;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DominoesProperties
{
    public class Program
    {
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            // Run the migrations
            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    Log.Information("Running database migrations...");
                    UpdateDatabase(serviceProvider);
                    Log.Information("Database migrations completed");
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .WriteTo.Console()
                        .WriteTo.File("logs/dp-.txt", rollingInterval: RollingInterval.Day)
                        .Enrich.FromLogContext()
                        .ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DominoProps_String");

                    services.AddLogging(c => c.AddFluentMigratorConsole())
                        .Configure<FluentMigratorLoggerOptions>(o =>
                        {
                            o.ShowSql = true;
                            o.ShowElapsedTime = true;
                        })
                        .AddFluentMigratorCore()
                        .ConfigureRunner(runner => runner
                            .AddMySql8()
                            .WithGlobalConnectionString(connectionString)
                            .ScanIn(typeof(Program).Assembly).For.Migrations());
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}