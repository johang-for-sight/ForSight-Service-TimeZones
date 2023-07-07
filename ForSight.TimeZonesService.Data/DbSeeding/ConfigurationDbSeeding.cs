using ForSight.TimeZonesService.Data.Entities;

namespace ForSight.TimeZonesService.Data.DbSeeding
{
    public static class ConfigurationDbSeeding
    {
        public static IEnumerable<ActiveTimeZone> GetActiveTimeZones() =>
            new List<ActiveTimeZone>()
            {
                new()
                {
                    ZoneId = "Europe/London",
                    IsActive = true
                }
            };
    }
}
