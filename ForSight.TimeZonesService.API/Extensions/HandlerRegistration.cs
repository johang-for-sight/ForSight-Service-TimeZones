using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Authorization;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Handlers.Query.GetActiveTimeZones;
using ForSight.TimeZonesService.Handlers.Query.Shared;
using ForSight.Utils.Extensions;

namespace ForSight.TimeZonesService.API.Extensions
{
    public static class HandlerRegistration
    {
        public static void RegisterHandlers(this IServiceCollection services)
        {
            services.RegisterAuthorizationHandlers();
            services.RegisterQueryBuilders();
            services.RegisterQueryHandlers();
            services.RegisterCommandHandlers();
        }

        private static void RegisterAuthorizationHandlers(this IServiceCollection services)
        {

        }

        private static void RegisterQueryBuilders(this IServiceCollection services)
        {
            services.AddScoped<IRequestContext<ITimeZonesServiceDbContext>, RequestContext<TimeZonesServiceDbContext>>();
            services.AddUniqueTransient<IAuthorizedQueryBuilder<ActiveTimeZone>, AuthorizedReadActiveTimeZonesQueryBuilder>();
            
        }

        private static void RegisterQueryHandlers(this IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetActiveTimeZonesQuery, List<GetActiveTimeZonesQueryResult>>, GetActiveTimeZonesQueryHandler>();
        }

        private static void RegisterCommandHandlers(this IServiceCollection services)
        {

        }
    }
}
