using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Handlers.Query.GetAllCampaignTypes;
using ForSight.TimeZonesService.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ForSight.TimeZonesService.Handlers.Query.GetAllTransactionTypes;

namespace ForSight.TimeZonesService.Tests.Handlers.Query
{
    public class GetAllTransactionTypesQueryHandlerTests
    {
        private readonly GetAllTransactionTypesQueryHandler _queryHandler;
        private readonly Mock<IAuthorizedQueryBuilder<TransactionType>> _authorizedQueryBuilderMock;
        private readonly List<TransactionType> _transactionTypes = new();
       
        public GetAllTransactionTypesQueryHandlerTests()
        {
            var mockSet = DbSetMockSetup.SetupMockDbSet(_transactionTypes);
            var dbContextMock = new Mock<ICampaignServiceDbContext>();
            dbContextMock.Setup(_ => _.TransactionTypes.AsQueryable()).Returns(mockSet.Object);
            var requestContextMock = new Mock<IRequestContext<ICampaignServiceDbContext>>();

            _authorizedQueryBuilderMock = new Mock<IAuthorizedQueryBuilder<TransactionType>>();
            _authorizedQueryBuilderMock.Setup(s => s.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()))
                .Returns(mockSet.Object);

            var logger = Mock.Of<ILogger<GetAllTransactionTypesQueryHandler>>();

            _queryHandler = new GetAllTransactionTypesQueryHandler(requestContextMock.Object, _authorizedQueryBuilderMock.Object, logger);
        }

        [Fact]
        public async Task Get_ValidQuery_ReturnsAllTransactionTypes()
        {
            // Arrange
            var query = new GetAllTransactionTypesQuery();
            CreateTransactionType(1, 1, "Stay");
            CreateTransactionType(2, 3, "Event");
            CreateTransactionType(3, 4, "Leisure");
            
            const int expectedTransactionTypeCount = 3;

            // Act
            var result = await _queryHandler.Get(query);

            // Assert
            Assert.Equal(expectedTransactionTypeCount, result.Count);
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
            var query = new GetAllTransactionTypesQuery();

            // Act
            await _queryHandler.Get(query);

            // Assert
            _authorizedQueryBuilderMock.Verify(x =>
                x.ComposeQuery(It.IsAny<IRequestContext<ICampaignServiceDbContext>>()), Times.Once);
        }

        private TransactionType CreateTransactionType(int id, int forSightTransactionTypeId, string transactionTypeDescription)
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
