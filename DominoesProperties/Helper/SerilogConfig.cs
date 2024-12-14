using Serilog;

namespace DominoesProperties.Helper
{
    public static class SerilogConfig
    {
        public static Serilog.ILogger Configure()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log/dominoes-properties.txt", rollingInterval: RollingInterval.Day).CreateLogger();

            return Log.Logger;
        }
    }
}