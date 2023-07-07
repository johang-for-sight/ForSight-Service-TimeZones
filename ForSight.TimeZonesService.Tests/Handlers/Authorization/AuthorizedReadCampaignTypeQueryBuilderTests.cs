using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AuthorizedReadCampaignTypeQueryBuilderTests
    {
        private readonly AuthorizedReadCampaignTypeQueryBuilder _queryBuilder;
        private readonly ClaimsIdentity _currentUserIdentity = new();
        private readonly Mock<IRequestContext<ICampaignServiceDbContext>> _requestContextMock;
        private readonly List<CampaignType> _campaignTypes = new();

        public AuthorizedReadCampaignTypeQueryBuilderTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_campaignTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.CampaignTypes.AsQueryable()).Returns(mockSet.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var currentUser = SetupCurrentUser();

            _requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();
            _requestContextMock.Setup(s => s.User).Returns(currentUser);
            _requestContextMock.Setup(s => s.DbContext).Returns(dbContextMock.Object);

            _queryBuilder = new AuthorizedReadCampaignTypeQueryBuilder();
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
        public void ComposeQuery_UserHasValidForSightUserRole_ReturnsAllCampaignTypes(string validRole)
        {
            // Arrange
            var userRoleClaim = new Claim(JwtClaimTypes.Role, validRole);
            _currentUserIdentity.AddClaims(new[] { userRoleClaim });
            CreateCampaignType(1, "Marketing");
            CreateCampaignType(2, "Transactional");
            var expectedCampaignTypeCount = 2;

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Equal(expectedCampaignTypeCount, result.Count());
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveForSightUserRole_ReturnsEmptyResult()
        {
            // Arrange
            const string userRole = "InvalidUserRole";
            var invalidRoleClaim = new Claim(JwtClaimTypes.Role, userRole);
            _currentUserIdentity.AddClaims(new[] { invalidRoleClaim });
            CreateCampaignType(1, "Marketing");

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveRoleClaim_ReturnsEmptyResult()
        {
            // Arrange
            CreateCampaignType(1, "Marketing");

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


        private CampaignType CreateCampaignType(int id, string name)
        {
            var labels = CreateCampaignTypeLabels(id, name);
            var campaignType = new CampaignType
            {
                Id = id,
                Type = name,
                Description = name,
                ActiveIcon = name,
                InactiveIcon = name,
                SortOrder = id,
                CampaignTypeLabels = new List<CampaignTypeLabel> { labels }
            };
            _campaignTypes.Add(campaignType);
            return campaignType;
        }

       
        private static CampaignTypeLabel CreateCampaignTypeLabels(int id, string name)
        {
            var campaignTypeLabel = new CampaignTypeLabel
            {
                Id = id,
                CampaignTypeId = id,
                Text = name,
                ToolTip = name,
                SortOrder = 1
            };
            return campaignTypeLabel;
        }
    }
}
