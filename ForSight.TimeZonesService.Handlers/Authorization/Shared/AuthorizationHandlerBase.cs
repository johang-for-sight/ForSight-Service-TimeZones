using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForSight.Authorization;
using ForSight.Utils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ForSight.TimeZonesService.Handlers.Authorization.Shared
{
    public abstract class AuthorizationHandlerBase<TRequirement>
        : IAuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _currentUser;

        protected AuthorizationHandlerBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUser = _httpContextAccessor.HttpContext?.User ?? throw new InvalidOperationException("HttpContext.User is null.");
        }

        public async Task<bool> HandleRequirement(TRequirement requirement)
        {
            Error.ThrowIfNull(requirement, nameof(requirement));

            // Originally coming from MS :AuthorizationHandler<XRequirement> when inheriting that class.
            var authorizationHandlerContext = new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement> { requirement },
                _currentUser,
                null);

            await HandleRequirement(authorizationHandlerContext, requirement);

            return authorizationHandlerContext.HasSucceeded;
        }

        protected abstract Task HandleRequirement(AuthorizationHandlerContext context, TRequirement requirement);

        protected bool CheckIsCustomerAdministrator(int customerId)
        {
            var hasAdminRole = _currentUser.HasRole(RoleNames.Administrator);

            var customerClaims = _currentUser.FindAll(UserCustomClaimTypes.UserCustomerId);

            var hasCustomerAccess = customerClaims.Any(c =>
            {
                if (int.TryParse(c.Value, out var customerIdClaim))
                {
                    return customerIdClaim == customerId;
                }

                return false;
            });

            return hasAdminRole && hasCustomerAccess;
        }
    }
}