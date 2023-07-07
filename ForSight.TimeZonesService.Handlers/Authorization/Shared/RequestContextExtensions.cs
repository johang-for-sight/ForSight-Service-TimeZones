using ForSight.Authorization;
using ForSight.TimeZonesService.Data;
using IdentityModel;

namespace ForSight.TimeZonesService.Handlers.Authorization.Shared
{
    public static class RequestContextExtensions
    {
        public static bool RequestUserIsForSightUser(this IRequestContext<ITimeZonesServiceDbContext> context)
        {
            var user = context.User;
            return user.HasClaim(JwtClaimTypes.Role, RoleNames.ForthAdministrator)
                  || user.HasClaim(JwtClaimTypes.Role, RoleNames.Administrator)
                  || user.HasClaim(JwtClaimTypes.Role, RoleNames.User)
                  || user.HasClaim(JwtClaimTypes.Role, RoleNames.FrontDeskUser);
        }
    }
}
