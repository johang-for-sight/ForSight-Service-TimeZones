using AutoMapper;
using ForSight.TimeZonesService.API.AutoMapper;
using Xunit;

namespace ForSight.TimeZonesService.Tests.Mapping
{
    public class CampaignGoalMappingProfileTests
    {
        private readonly IMapper _mapper;

        public CampaignGoalMappingProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<CampaignGoalMappingProfile>()).CreateMapper();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
