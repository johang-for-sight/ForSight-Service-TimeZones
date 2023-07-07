using AutoMapper;
using ForSight.TimeZonesService.API.AutoMapper;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Mapping
{
    public class TransactionTypeMappingProfileTests
    {
        private readonly IMapper _mapper;

        public TransactionTypeMappingProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<TransactionTypeMappingProfile>()).CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
