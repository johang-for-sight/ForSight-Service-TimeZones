using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using ForSight.TimeZonesService.Handlers.Query.GetAllAudienceTypes;
using Xunit;
using ForSight.TimeZonesService.API.Models;

namespace ForSight.TimeZonesService.Tests.Handlers.Query
{
    public class GetAllAudienceTypesQueryHandlerTests
    {
        private readonly GetAudienceTypesQueryHandler _queryHandler;
        private readonly Mock<IAuthorizedQueryBuilder<AudienceType>> _authorizedQueryBuilderMock;
        private readonly List<AudienceType> _audienceTypes = new();

        public GetAllAudienceTypesQueryHandlerTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_audienceTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.AudienceTypes.AsQueryable()).Returns(mockSet.Object);
            var requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();

            _authorizedQueryBuilderMock = new Mock<IAuthorizedQueryBuilder<AudienceType>>();
            _authorizedQueryBuilderMock.Setup(s => s.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()))
                .Returns(mockSet.Object);

            var logger = Mock.Of<ILogger<GetAudienceTypesQueryHandler>>();

            _queryHandler = new GetAudienceTypesQueryHandler(requestContextMock.Object, _authorizedQueryBuilderMock.Object, logger);
        }

        [Fact]
        public async Task Get_AudienceTypesExistForCampaignTypeId_ReturnsAudienceTypes()
        {
            // Arrange
            var audienceType1 = CreateAudienceType(1, 1, "Audience 1");
            var audienceType2 = CreateAudienceType(2, 1, "Audience 2");
            var expectedResult = new List<GetAudienceTypesResultDto>()
            {
                new GetAudienceTypesResultDto()
                {
                    Id = audienceType1.Id,
                    Type = audienceType1.Type,
                    Description = audienceType1.Description,
                    SortOrder = audienceType1.SortOrder,
                    ActiveIcon = audienceType1.ActiveIcon,
                    InactiveIcon = audienceType1.InactiveIcon
                },
                new GetAudienceTypesResultDto()
                {
                    Id = audienceType2.Id,
                    Type = audienceType2.Type,
                    Description = audienceType2.Description,
                    SortOrder = audienceType2.SortOrder,
                    ActiveIcon = audienceType2.ActiveIcon,
                    InactiveIcon = audienceType2.InactiveIcon
                }
            };
            var query = new GetAudienceTypesQuery(audienceType1.CampaignTypeId);

            // Act
            var actualResult = await _queryHandler.Get(query);

            // Assert
            Assert.Equal(expectedResult.Count, actualResult.Count);
        }

        [Fact]
        public async Task Get_AudienceTypeDoesNotExistForCampaignTypeId_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAudienceTypesQuery(-1);

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
            var audienceType = CreateAudienceType(1, 1, "Audience");
            var query = new GetAudienceTypesQuery(audienceType.CampaignTypeId);

            // Act
            await _queryHandler.Get(query);

            // Assert
            _authorizedQueryBuilderMock.Verify(x =>
                x.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()), Times.Once);
        }

        private AudienceType CreateAudienceType(int id, int campaignTypeId, string name)
        {
            var audienceType = new AudienceType()
            {
                Id = id,
                CampaignTypeId = campaignTypeId,
                Type = name,
                Description = name,
                SortOrder = 1,
                ActiveIcon = "ActiveIcon",
                InactiveIcon = "InactiveIcon"
            };
            _audienceTypes.Add(audienceType);
            return audienceType;
        }
    }
}
