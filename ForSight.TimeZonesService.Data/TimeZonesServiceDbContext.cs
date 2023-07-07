using System;
using ForSight.Audit;
using ForSight.TimeZonesService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ForSight.TimeZonesService.Data
{
    public class TimeZonesServiceDbContext : DbContext, ITimeZonesServiceDbContext
    {
        private readonly ICurrentUserContext _currentUserContext;

        public TimeZonesServiceDbContext(
            DbContextOptions<TimeZonesServiceDbContext> options,
            ICurrentUserContext currentUserContext)
            : base(options)
        {
            _currentUserContext = currentUserContext;
        }

        public virtual DbSet<AuditRecord> AuditRecords { get; set; }
        public virtual DbSet<ActiveTimeZone> ActiveTimeZones{ get; set; }


        public override int SaveChanges()
        {
            var requestUserId = _currentUserContext.RequestUserId;
            if (requestUserId != Guid.Empty)
            {
                var entries = ChangeTracker.Entries<IAuditable>();
                this.AuditChanges<AuditRecord>(requestUserId, entries).GetAwaiter().GetResult();
                
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var requestUserId = _currentUserContext.RequestUserId;
            if (requestUserId != Guid.Empty)
            {
                var entries = ChangeTracker.Entries<IAuditable>();
                await this.AuditChanges<AuditRecord>(requestUserId, entries);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return base.Database.BeginTransactionAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureAudit();
            modelBuilder.ConfigureActiveTimeZones();
            
        }
    }
}
