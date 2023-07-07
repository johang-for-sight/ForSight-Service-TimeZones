using System.Security.Claims;
using ForSight.Utils.Core;
using Microsoft.AspNetCore.Http;

namespace ForSight.TimeZonesService.Handlers.Authorization.Shared
{
    public class RequestContext<TDbContext> : IRequestContext<TDbContext>
    {
        public ClaimsPrincipal User { get; }

        public TDbContext DbContext { get; }

        public RequestContext(IHttpContextAccessor contextAccessor, TDbContext dbContext)
        {
            Error.ThrowIfNull(contextAccessor, nameof(contextAccessor));
            Error.ThrowIfNull(dbContext, nameof(dbContext));

            User = contextAccessor.HttpContext?.User ?? throw new InvalidOperationException("HttpContext.User is null.");

            DbContext = dbContext;
        }
    }
}