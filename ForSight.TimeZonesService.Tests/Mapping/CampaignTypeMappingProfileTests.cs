using AutoMapper;
using ForSight.TimeZonesService.API.AutoMapper;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Mapping
{
    public class CampaignTypeMappingProfileTests
    {
        private readonly IMapper _mapper;

        public CampaignTypeMappingProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<CampaignTypeMappingProfile>()).CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
