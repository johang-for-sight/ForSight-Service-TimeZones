using Destructurama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ForSight.TimeZonesService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSetting("detailedErrors", "true");
                    webBuilder.CaptureStartupErrors(true);
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.Configure(options =>
                    {
                        options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId;
                    });
                })
                .UseSerilog((context, loggerConfig) =>
                    loggerConfig.ReadFrom
                        .Configuration(context.Configuration)
                        .Destructure.UsingAttributes()
                );
    }
}
