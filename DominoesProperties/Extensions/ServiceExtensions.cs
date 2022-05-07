using System;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Repository;
using Repositories.Service;

namespace DominoesProperties.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureNLogService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
    }
}
