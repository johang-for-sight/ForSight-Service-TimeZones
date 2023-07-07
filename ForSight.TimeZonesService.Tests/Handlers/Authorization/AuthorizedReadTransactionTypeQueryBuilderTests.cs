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
    public class AuthorizedReadTransactionTypeQueryBuilderTests
    {
        private readonly AuthorizedReadTransactionTypeQueryBuilder _queryBuilder;
        private readonly ClaimsIdentity _currentUserIdentity = new();
        private readonly Mock<IRequestContext<ICampaignServiceDbContext>> _requestContextMock;
        private readonly List<TransactionType> _transactionTypes = new();

        public AuthorizedReadTransactionTypeQueryBuilderTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_transactionTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.TransactionTypes.AsQueryable()).Returns(mockSet.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var currentUser = SetupCurrentUser();

            _requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();
            _requestContextMock.Setup(s => s.User).Returns(currentUser);
            _requestContextMock.Setup(s => s.DbContext).Returns(dbContextMock.Object);

            _queryBuilder = new AuthorizedReadTransactionTypeQueryBuilder();
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
        public void ComposeQuery_UserHasValidForSightUserRole_ReturnsAllTransactionTypes(string validRole)
        {
            // Arrange
            var userRoleClaim = new Claim(JwtClaimTypes.Role, validRole);
            _currentUserIdentity.AddClaims(new[] { userRoleClaim });
            CreateTransactionType(1,1, "Stay");
            CreateTransactionType(2,3, "Leisure");
            CreateTransactionType(3, 4, "Event");
            var expectedTransactionTypeCount = 3;

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Equal(expectedTransactionTypeCount, result.Count());
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveForSightUserRole_ReturnsEmptyResult()
        {
            // Arrange
            const string userRole = "InvalidUserRole";
            var invalidRoleClaim = new Claim(JwtClaimTypes.Role, userRole);
            _currentUserIdentity.AddClaims(new[] { invalidRoleClaim });
            CreateTransactionType(1, 1, "Stay");

            // Act
            var result = _queryBuilder.ComposeQuery(_requestContextMock.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ComposeQuery_UserDoesNotHaveRoleClaim_ReturnsEmptyResult()
        {
            // Arrange
            CreateTransactionType(1, 1, "Stay");

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


        private TransactionType CreateTransactionType(int id,int forSightTransactionTypeId, string transactionTypeDescription)
        {
            var transactionType = new TransactionType
            {
                Id = id,
                ForSightTransactionTypeId = forSightTransactionTypeId,
                TransactionTypeDescription = transactionTypeDescription,
            };
            _transactionTypes.Add(transactionType);
            return transactionType;
        }
    }
}
