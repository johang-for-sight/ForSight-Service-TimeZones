using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;

namespace ForSight.TimeZonesService.Handlers.Authorization
{
    public class AuthorizedReadActiveTimeZonesQueryBuilder : IAuthorizedQueryBuilder<ActiveTimeZone>
    {
        public IQueryable<ActiveTimeZone> ComposeQuery(IRequestContext<ITimeZonesServiceDbContext> requestContext)
        {
            var activeTimeZone = requestContext.DbContext.ActiveTimeZones.AsQueryable();
            var isForSightUser = requestContext.RequestUserIsForSightUser();
            if (isForSightUser)
            {
                return activeTimeZone;
            }

            return Enumerable.Empty<ActiveTimeZone>().AsQueryable();
        }
    }
}
