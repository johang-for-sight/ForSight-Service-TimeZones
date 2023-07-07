using ForSight.TimeZonesService.Data.Entities;
using ForSight.TimeZonesService.Handlers.Query.Shared;

namespace ForSight.TimeZonesService.Handlers.Query.GetActiveTimeZones
{
    public class GetActiveTimeZonesQueryResult : IQueryResult
    {
        public string ZoneId { get; set; } = string.Empty;
        public int Offset { get; set; }
        public int DaylightOffset { get; set; }
    }
}
