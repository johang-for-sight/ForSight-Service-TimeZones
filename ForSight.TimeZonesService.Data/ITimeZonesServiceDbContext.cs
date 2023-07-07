using System;
using ForSight.TimeZonesService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ForSight.TimeZonesService.Data
{
    public interface ITimeZonesServiceDbContext
    {
        DbSet<AuditRecord> AuditRecords { get; set; }

        DbSet<ActiveTimeZone> ActiveTimeZones { get; set; }
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
