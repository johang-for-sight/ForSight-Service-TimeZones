using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForSight.Audit;

namespace ForSight.TimeZonesService.Data.Entities
{
    public class AuditRecord : IAuditRecord
    {
        public int Id { get; set; }

        public string EntityId { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;

        public string PreviousValue { get; set; } = string.Empty;

        public string CurrentValue { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;

        public DateTimeOffset CreatedOn { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }

        public Guid ModifiedByUserId { get; set; }
    }
}
