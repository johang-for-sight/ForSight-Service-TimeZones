using ForSight.Authorization;
using ForSight.TimeZonesService.Core.Authorization;
using ForSight.TimeZonesService.Core.Config;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace ForSight.TimeZonesService.API.Extensions
{
    public static class AuthorizationRegistration
    {
        public static void AddAuthorizationWithPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {   
                var readScope = $"{ForSightApiResources.TimeZones}.{ClientScopes.ReadOnly}"; 
                var manageScope = $"{ForSightApiResources.TimeZones}.{ClientScopes.Manage}"; 

                options.AddPolicy(PolicyName.TimeZonesReadAllAccess, policy =>
                {
                    policy.RequireClaim("scope", new[] { readScope, manageScope });
                    var permittedClientIds = PermittedReadAllTimeZonesClients.ClientIds;
                    policy.RequireAssertion(ctx =>
                    {
                        var clientIdClaim = ctx.User.FindFirst(c => c.Type == JwtClaimTypes.ClientId);
                        if (clientIdClaim != null && permittedClientIds.Contains(clientIdClaim.Value))
                        {
                            return true;
                        }
                        else if (ctx.User.HasClaim(c => c.Type == "scope" && c.Value == manageScope))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                });

                // FallbackPolicy will be used when there is no [Authorize] attribute added to controller/endpoint
                // This allows us to set 'default' to most secure - Manage access and override where necessary with [Authorize]
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireClaim("scope", manageScope)
                    .Build();
            });
        }
    }
}
