using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using ForSight.TimeZonesService.Handlers.Query.GetAllCampaignGoals;
using Xunit;
using ForSight.TimeZonesService.API.Models;

namespace ForSight.TimeZonesService.Tests.Handlers.Query
{
    public class GetCampaignGoalsQueryHandlerTests
    {
        private readonly GetCampaignGoalsQueryHandler _queryHandler;
        private readonly Mock<IAuthorizedQueryBuilder<CampaignGoal>> _authorizedQueryBuilderMock;
        private readonly List<CampaignGoal> _campaignGoals = new();

        public GetCampaignGoalsQueryHandlerTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_campaignGoals);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.CampaignGoals.AsQueryable()).Returns(mockSet.Object);
            var requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();

            _authorizedQueryBuilderMock = new Mock<IAuthorizedQueryBuilder<CampaignGoal>>();
            _authorizedQueryBuilderMock.Setup(s => s.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()))
                .Returns(mockSet.Object);

            var logger = Mock.Of<ILogger<GetCampaignGoalsQueryHandler>>();

            _queryHandler = new GetCampaignGoalsQueryHandler(requestContextMock.Object, _authorizedQueryBuilderMock.Object, logger);
        }

        [Fact]
        public async Task Get_CampaignGoalsExistForCampaignTypeId_ReturnsCampaignGoals()
        {
            // Arrange
            var campaignGoal1 = CreateCampaignGoal(1, 1, 1, "Goal 1");
            var campaignGoal2 = CreateCampaignGoal(2, 1, 2, "Goal 2");
            var expectedResult = new List<GetCampaignGoalsResultDto>()
            {
                new GetCampaignGoalsResultDto()
                {
                    Id = campaignGoal1.Id,
                    Name = campaignGoal1.Goal.Name,
                    SortOrder = campaignGoal1.SortOrder
                },
                new GetCampaignGoalsResultDto()
                {
                    Id = campaignGoal2.Id,
                    Name = campaignGoal2.Goal.Name,
                    SortOrder = campaignGoal2.SortOrder
                }
            };
            var query = new GetCampaignGoalsQuery(campaignGoal1.CampaignTypeId);

            // Act
            var actualResult = await _queryHandler.Get(query);

            // Assert
            Assert.Equal(expectedResult.Count, actualResult.Count);
        }

        [Fact]
        public async Task Get_CampaignGoalDoesNotExistForCampaignTypeId_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetCampaignGoalsQuery(-1);

            // Act
            var result = await _queryHandler.Get(query);

            //Assert
            Assert.Empty(result);
        }


        [Fact]
        public async Task Get_NullQuery_ThrowsArgumentNullException()
        {
            // Arrange

            // Act/Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is being disabled in order that we can test for Null exception.
            await Assert.ThrowsAsync<ArgumentNullException>(() => _queryHandler.Get(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public async Task Get_ValidQuery_CallsAuthorizedQueryBuilder()
        {
            // Arrange
            var campaignGoal = CreateCampaignGoal(1, 1, 2, "Goal");
            var query = new GetCampaignGoalsQuery(campaignGoal.CampaignTypeId);

            // Act
            await _queryHandler.Get(query);

            // Assert
            _authorizedQueryBuilderMock.Verify(x =>
                x.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()), Times.Once);
        }

        private CampaignGoal CreateCampaignGoal(int id, int campaignTypeId, int goalId, string name)
        {
            var campaignGoal = new CampaignGoal
            {
                Id = id,
                CampaignTypeId = campaignTypeId,
                GoalId = goalId,
                SortOrder = id,
                Goal = new Goal { Name = name }
            };
            _campaignGoals.Add(campaignGoal);
            return campaignGoal;
        }
    }
}
