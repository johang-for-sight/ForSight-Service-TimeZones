using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForSight.TimeZonesService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForSight.TimeZonesService.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureAudit(this ModelBuilder builder)
        {
            builder.Entity<AuditRecord>().HasKey(audit => audit.Id);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.EntityId).HasMaxLength(50);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.EntityName).HasMaxLength(50);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.PropertyName).HasMaxLength(50);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.PreviousValue).HasMaxLength(255);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.CurrentValue).HasMaxLength(255);
            builder.Entity<AuditRecord>().Property(auditRecord => auditRecord.EventType).HasMaxLength(50);
        }

        public static void ConfigureActiveTimeZones(this ModelBuilder builder)
        {
            builder.Entity<ActiveTimeZone>().HasKey(type => type.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.None);
            builder.Entity<ActiveTimeZone>().Property(type => type.ZoneId).IsRequired();
            builder.Entity<ActiveTimeZone>().Property(type => type.IsActive).IsRequired();
            
        }
      
    }
}
