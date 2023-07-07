using ForSight.TimeZonesService.API.AppInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace ForSight.TimeZonesService.API.Extensions
{
    public static class AppInsightsRegistration
    {
        public static void AddApplicationInsights(this IServiceCollection services)
        {
            // Enables Application Insights telemetry collection
            services.AddApplicationInsightsTelemetry();

            // Adds TelemetryInitializers to enrich telemetry with additional information
            services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, CurrentUserIdTelemetryInitializer>();
        }
    }
}
