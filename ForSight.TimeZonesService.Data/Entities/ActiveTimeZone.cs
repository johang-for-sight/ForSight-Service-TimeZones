using System.ComponentModel.DataAnnotations.Schema;
using ForSight.Audit;

namespace ForSight.TimeZonesService.Data.Entities
{
    public class ActiveTimeZone:IAuditable
    {
        public int Id { get; set; }
        public string ZoneId { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        
    }
}
