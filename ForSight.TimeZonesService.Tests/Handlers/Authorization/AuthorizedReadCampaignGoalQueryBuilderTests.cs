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
    public class AuthorizedReadCampaignGoalQueryBuilderTests
    {
        private readonly AuthorizedReadCampaignGoalQueryBuilder _queryBuilder;
        private readonly ClaimsIdentity _currentUserIdentity = new();
        private readonly Mock<IRequestContext<ICampaignServiceDbContext>> _requestContextMock;
        private readonly List<CampaignGoal> _campaignGoals = new();

        public AuthorizedReadCampaignGoalQueryBuilderTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_campaignGoals);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.CampaignGoals.AsQueryable()).Returns(mockSet.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var currentUser = SetupCurrentUser();

            _requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();
            _requestContextMock.Setup(s => s.User).Returns(currentUser);
            _requestContextMock.Setup(s => s.DbContext).Returns(dbContextMock.Object);

            _queryBuilder = new AuthorizedReadCampaignGoalQueryBuilder();
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
        public void ComposeQuery_UserHasValidForSightUserRole_ReturnsAllCampaignGoals(string validRole)
        {
            // Arrange
            var userRoleClaim = new Claim(JwtClaimTypes.Role, validRole);
            _currentUserIdentity.AddClaims(new[] { userRoleClaim });
            CreateCampaignGoal(1, 1, "Goal 1");
            CreateCampaignGoal(2, 1, "Goal 2");
            var expectedCampaignGoalCount = 2;

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Equal(expectedCampaignGoalCount, result.Count());
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveForSightUserRole_ReturnsEmptyResult()
        {
            // Arrange
            const string userRole = "InvalidUserRole";
            var invalidRoleClaim = new Claim(JwtClaimTypes.Role, userRole);
            _currentUserIdentity.AddClaims(new[] { invalidRoleClaim });
            CreateCampaignGoal(1, 1, "Goal");

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveRoleClaim_ReturnsEmptyResult()
        {
            // Arrange
            CreateCampaignGoal(1, 1, "Goal");

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

        private CampaignGoal CreateCampaignGoal(int id, int campaignTypeId, string name)
        {
            var campaignGoal = new CampaignGoal
            {
                Id = id,
                CampaignTypeId = campaignTypeId,
                SortOrder = id,
                Goal = new Goal
                {
                    Name = name
                }
            };
            _campaignGoals.Add(campaignGoal);
            return campaignGoal;
        }
    }
}
