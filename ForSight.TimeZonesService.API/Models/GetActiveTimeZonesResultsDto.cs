namespace ForSight.TimeZonesService.API.Models
{
    public class GetActiveTimeZonesResultsDto
    {
        public string ZoneId { get; set; } = string.Empty;
        public int Offset { get; set; } = 0;
        public int DaylightOffset { get; set; } = 0;
    }
}
