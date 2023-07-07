using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace ForSight.TimeZonesService.API.AppInsights
{
    public class CurrentUserIdTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserIdTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            if (userName != null)
            {
                telemetry.Context.User.Id = userName;
            }
        }
    }
}
