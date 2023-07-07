using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Handlers.Query.GetAllCampaignTypes;
using ForSight.TimeZonesService.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Handlers.Query
{
    public class GetAllCampaignTypesQueryHandlerTests
    {
        private readonly GetAllCampaignTypesQueryHandler _queryHandler;
        private readonly Mock<IAuthorizedQueryBuilder<CampaignType>> _authorizedQueryBuilderMock;
        private readonly List<CampaignType> _campaignTypes = new();

        public GetAllCampaignTypesQueryHandlerTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_campaignTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.CampaignTypes.AsQueryable()).Returns(mockSet.Object);
            var requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();

            _authorizedQueryBuilderMock = new Mock<IAuthorizedQueryBuilder<CampaignType>>();
            _authorizedQueryBuilderMock.Setup(s => s.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()))
                .Returns(mockSet.Object);

            var logger = Mock.Of<ILogger<GetAllCampaignTypesQueryHandler>>();

            _queryHandler = new GetAllCampaignTypesQueryHandler(requestContextMock.Object, _authorizedQueryBuilderMock.Object, logger);
        }

        [Fact]
        public async Task Get_ValidQuery_ReturnsAllCampaignTypes()
        {
            // Arrange
            var query = new GetAllCampaignTypesQuery();
            CreateCampaignType(1, "Marketing");
            CreateCampaignType(2, "Transactional");
            const int expectedCampaignTypeCount = 2;

            // Act
            var result = await _queryHandler.Get(query);

            // Assert
            Assert.Equal(expectedCampaignTypeCount, result.Count);
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
            var query = new GetAllCampaignTypesQuery();

            // Act
            await _queryHandler.Get(query);

            // Assert
            _authorizedQueryBuilderMock.Verify(x =>
                x.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()), Times.Once);
        }

        private CampaignType CreateCampaignType(int id, string name)
        {
            var campaignType = new CampaignType
            {
                Id = id,
                Type = name
            };
            _campaignTypes.Add(campaignType);
            return campaignType;
        }
      
    }
}
