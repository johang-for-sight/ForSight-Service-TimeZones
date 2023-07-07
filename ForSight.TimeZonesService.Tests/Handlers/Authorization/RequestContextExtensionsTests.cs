using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForSight.Authorization;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Handlers.Authorization
{
    public class RequestContextExtensionsTests
    {
        private readonly DefaultHttpContext _httpContext;
        private readonly ClaimsIdentity _identity;
        private readonly IRequestContext<ICampaignServiceDbContext> _requestContext;

        public RequestContextExtensionsTests()
        {
            _httpContext = new DefaultHttpContext();
            _identity = new ClaimsIdentity();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(_httpContext);

            SetupRequestUser();

            var mockDbContext = new Mock<ICampaignServiceDbContext>();

            _requestContext = new RequestContext<ICampaignServiceDbContext>(mockHttpContextAccessor.Object, mockDbContext.Object);
        }

        public static TheoryData<string> AllForSightRoles =>
            new()
            {
                RoleNames.ForthAdministrator,
                RoleNames.Administrator,
                RoleNames.User,
                RoleNames.FrontDeskUser
            };

        [Theory]
        [MemberData(nameof(AllForSightRoles))]
        public void RequestUserIsForSightUser_UserHasForSightRole_ReturnsTrue(string forSightRole)
        {
            // Arrange
            var roleClaim = new Claim(JwtClaimTypes.Role, forSightRole);
            _identity.AddClaim(roleClaim);

            // Act
            var isForSightUser = _requestContext.RequestUserIsForSightUser();

            // Assert
            Assert.True(isForSightUser);

        }

        [Fact]
        public void RequestUserIsForSightUser_UserDoesNotHaveForSightRole_ReturnsFalse()
        {
            // Arrange
            var roleClaim = new Claim(JwtClaimTypes.Role, "InvalidRole");
            _identity.AddClaim(roleClaim);

            // Act
            var isForSightUser = _requestContext.RequestUserIsForSightUser();

            // Assert
            Assert.False(isForSightUser);
        }

        private void SetupRequestUser()
        {
            var userId = Guid.NewGuid();
            var subClaim = new Claim(JwtClaimTypes.Subject, userId.ToString());
            _identity.AddClaim(subClaim);

            var requestUser = new ClaimsPrincipal();
            requestUser.AddIdentity(_identity);

            _httpContext.User = requestUser;
        }
    }
}
