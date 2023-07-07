using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Handlers.Authorization
{
    public class RequestContextTests
    {
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ICampaignServiceDbContext> _campaignServiceDbContextMock;
        private readonly IRequestContext<ICampaignServiceDbContext> _requestContext;

        public RequestContextTests()
        {
            _httpContext = new DefaultHttpContext();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(_httpContext);

            SetupRequestUser();

            _campaignServiceDbContextMock = new Mock<ICampaignServiceDbContext>();
            _requestContext = new RequestContext<ICampaignServiceDbContext>(_httpContextAccessorMock.Object, _campaignServiceDbContextMock.Object);
        }

        [Fact]
        public void NewInstance_DbContextIsNull_ThrowsArgumentNullException()
        {
            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.  This is being disabled in order that we can test for Null exception.
            Assert.Throws<ArgumentNullException>(() =>
                new RequestContext<ICampaignServiceDbContext>(_httpContextAccessorMock.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void NewInstance_HttpContextAccessorIsNull_ThrowsArgumentNullException()
        {
            // Act / Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.  This is being disabled in order that we can test for Null exception.
            Assert.Throws<ArgumentNullException>(() =>
                new RequestContext<ICampaignServiceDbContext>(null, _campaignServiceDbContextMock.Object));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void NewInstance_HttpContextIsNull_ThrowsInvalidOperationException()
        {
            // Act / Assert
            Assert.Throws<InvalidOperationException>(() =>
                new RequestContext<ICampaignServiceDbContext>(new HttpContextAccessor(), _campaignServiceDbContextMock.Object));
        }

        [Fact]
        public void User_ReturnsHttpContextUser()
        {
            // Act
            var result = _requestContext.User;

            // Assert
            Assert.Equal(_httpContext.User, result);
        }

        [Fact]
        public void DbContext_ReturnsDbContext()
        {
            // Act
            var result = _requestContext.DbContext;

            // Assert
            Assert.Equal(_campaignServiceDbContextMock.Object, result);
        }

        private void SetupRequestUser()
        {
            var userId = Guid.NewGuid();
            var requestUserIdentity = new ClaimsIdentity();
            var subClaim = new Claim(JwtClaimTypes.Subject, userId.ToString());
            requestUserIdentity.AddClaim(subClaim);

            var requestUser = new ClaimsPrincipal();
            requestUser.AddIdentity(requestUserIdentity);

            _httpContext.User = requestUser;
        }
    }
}
