using System.Security.Claims;

namespace ForSight.TimeZonesService.Handlers.Authorization.Shared
{
    public interface IRequestContext<out TDbContext>
    {
        ClaimsPrincipal User { get; }

        TDbContext DbContext { get; }
    }
}