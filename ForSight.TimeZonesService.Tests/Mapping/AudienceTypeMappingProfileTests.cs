using AutoMapper;
using ForSight.TimeZonesService.API.AutoMapper;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Mapping
{
    public class AudienceTypeMappingProfileTests
    {
        private readonly IMapper _mapper;

        public AudienceTypeMappingProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AudienceTypeMappingProfile>()).CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
