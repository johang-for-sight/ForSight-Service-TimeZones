using ForSight.TimeZonesService.Core.Config;
using ForSight.TimeZonesService.Handlers.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForSight.TimeZonesService.API.Extensions
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IEncryptionService, EncryptionService>();

        }

        public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EncryptionOptions>(
                options => configuration.GetSection("EncryptionOptions").Bind(options));
        }
    }
}
