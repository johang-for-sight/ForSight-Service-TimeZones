using System.Security.Claims;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Authorization;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Tests.TestHelpers;
using ForSight.Authorization;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Handlers.Authorization
{
    public class AuthorizedReadAudienceTypeQueryBuilderTests
    {
        private readonly AuthorizedReadAudienceTypeQueryBuilder _queryBuilder;
        private readonly ClaimsIdentity _currentUserIdentity = new();
        private readonly Mock<IRequestContext<ICampaignServiceDbContext>> _requestContextMock;
        private readonly List<AudienceType> _audienceTypes = new();

        public AuthorizedReadAudienceTypeQueryBuilderTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_audienceTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.AudienceTypes.AsQueryable()).Returns(mockSet.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var currentUser = SetupCurrentUser();

            _requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();
            _requestContextMock.Setup(s => s.User).Returns(currentUser);
            _requestContextMock.Setup(s => s.DbContext).Returns(dbContextMock.Object);

            _queryBuilder = new AuthorizedReadAudienceTypeQueryBuilder();
        }

        public static TheoryData<string> ValidForSightRoles =>
            new()
            {
                RoleNames.ForthAdministrator,
                RoleNames.Administrator,
                RoleNames.User,
                RoleNames.FrontDeskUser
            };

        [Theory]
        [MemberData(nameof(ValidForSightRoles))]
        public void ComposeQuery_UserHasValidForSightUserRole_ReturnsAllAudienceTypes(string validRole)
        {
            // Arrange
            var userRoleClaim = new Claim(JwtClaimTypes.Role, validRole);
            _currentUserIdentity.AddClaims(new[] { userRoleClaim });
            CreateAudienceType(1, 1, "Audience 1");
            CreateAudienceType(2, 1, "Audience 2");
            var expectedAudienceTypeCount = 2;

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Equal(expectedAudienceTypeCount, result.Count());
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveForSightUserRole_ReturnsEmptyResult()
        {
            // Arrange
            const string userRole = "InvalidUserRole";
            var invalidRoleClaim = new Claim(JwtClaimTypes.Role, userRole);
            _currentUserIdentity.AddClaims(new[] { invalidRoleClaim });
            CreateAudienceType(1, 1, "Audience");

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveRoleClaim_ReturnsEmptyResult()
        {
            // Arrange
            CreateAudienceType(1, 1, "Audience");

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Empty(result);
        }


        private ClaimsPrincipal SetupCurrentUser()
        {
            var userId = Guid.NewGuid();
            var subClaim = new Claim(JwtClaimTypes.Subject, userId.ToString());
            _currentUserIdentity.AddClaim(subClaim);

            var currentUser = new ClaimsPrincipal();
            currentUser.AddIdentity(_currentUserIdentity);
            return currentUser;
        }

        private AudienceType CreateAudienceType(int id, int campaignTypeId, string name)
        {
            var audienceType = new AudienceType
            {
                Id = id,
                CampaignTypeId = campaignTypeId,
                Type = name,
                Description = name,
                ActiveIcon = name,
                InactiveIcon = name,
                SortOrder = id
            };
            _audienceTypes.Add(audienceType);
            return audienceType;
        }
    }
}
