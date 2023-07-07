using AutoMapper;
using ForSight.TimeZonesService.API.Models;
using ForSight.TimeZonesService.Handlers.Query.GetActiveTimeZones;


namespace ForSight.TimeZonesService.API.AutoMapper
{
    public class AudienceTypeMappingProfile : Profile
    {
        public AudienceTypeMappingProfile()
        {
            CreateMap<GetActiveTimeZonesResultsDto, GetActiveTimeZonesQuery>();

        }
    }
}
