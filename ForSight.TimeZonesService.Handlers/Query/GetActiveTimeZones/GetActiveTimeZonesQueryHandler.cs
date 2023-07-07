using ForSight.TimeZonesService.Handlers.Authorization.Shared;
using ForSight.TimeZonesService.Handlers.Query.Shared;
using ForSight.TimeZonesService.Data;
using ForSight.TimeZonesService.Data.Entities;
using ForSight.Utils.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

namespace ForSight.TimeZonesService.Handlers.Query.GetActiveTimeZones;

public class GetActiveTimeZonesQueryHandler : QueryHandler<GetActiveTimeZonesQuery, List<GetActiveTimeZonesQueryResult>>
{
    private readonly IRequestContext<ITimeZonesServiceDbContext> _requestContext;
    private readonly IAuthorizedQueryBuilder<ActiveTimeZone> _authorizedQueryBuilder;
    private readonly ILogger<GetActiveTimeZonesQueryHandler> _logger;

    public GetActiveTimeZonesQueryHandler(
        IRequestContext<ITimeZonesServiceDbContext> requestContext,
        IAuthorizedQueryBuilder<ActiveTimeZone> authorizedQueryBuilder,
        ILogger<GetActiveTimeZonesQueryHandler> logger)
        : base(requestContext.DbContext)
    {
        _requestContext = requestContext;
        _authorizedQueryBuilder = authorizedQueryBuilder;
        _logger = logger;
    }

    protected override async Task<List<GetActiveTimeZonesQueryResult>> Handle(GetActiveTimeZonesQuery query)
    {
        Error.ThrowIfNull(query, nameof(query));
        _logger.LogDebug("Executing {@Query}", query);

        var queryable = _authorizedQueryBuilder.ComposeQuery(_requestContext);

        queryable = queryable.Where(c => c.IsActive==true);

        var resultActive = await queryable
            .Select(c => new GetActiveTimeZonesQueryResult
            {
                ZoneId = c.ZoneId,
            }).ToListAsync();

        return GetTimeZonesInfo(resultActive);
    }

    private List<GetActiveTimeZonesQueryResult> GetTimeZonesInfo(List<GetActiveTimeZonesQueryResult> activeZones)
    {

        IEnumerable<DateTimeZone> timeZones = DateTimeZoneProviders.Tzdb.GetAllZones();

        List<FSTimeZoneInfo> timeZoneList = new List<FSTimeZoneInfo>();

        var nowUtc = SystemClock.Instance.GetCurrentInstant();

        foreach (var timeZone in timeZones)
        {
            var currentOffset = timeZone.GetUtcOffset(nowUtc).ToTimeSpan().TotalHours;
            var daylightOffset = 0;

            var zoneInterval = timeZone.GetZoneInterval(nowUtc);

            if (zoneInterval.Savings != Offset.Zero)
            {
                var daylightSavingsStart = zoneInterval.Start;
                var daylightSavingsEnd = zoneInterval.End;

                if (nowUtc >= daylightSavingsStart && nowUtc < daylightSavingsEnd)
                {
                    currentOffset += zoneInterval.Savings.ToTimeSpan().TotalHours;
                }
                else
                {
                    var previousInterval = timeZone.GetZoneInterval(daylightSavingsStart.Minus(Duration.Epsilon));
                    var nextInterval = timeZone.GetZoneInterval(daylightSavingsEnd.Plus(Duration.Epsilon));
                    var previousOffset = previousInterval.Savings.ToTimeSpan().TotalHours;
                    var nextOffset = nextInterval.Savings.ToTimeSpan().TotalHours;

                    var offsetChange = nextOffset - previousOffset;

                    if (offsetChange != 0)
                    {
                        var timeUntilNextOffsetChange = nextInterval.Start - nowUtc;
                        var remainingOffset = offsetChange * (1 - timeUntilNextOffsetChange.TotalMilliseconds / nextInterval.Duration.TotalMilliseconds);
                        currentOffset += remainingOffset;
                    }
                }
                daylightOffset = (int)zoneInterval.Savings.ToTimeSpan().TotalHours;
            }

            FSTimeZoneInfo timeZoneInfo = new FSTimeZoneInfo(
                id: timeZone.Id,
                offset: (int)currentOffset,
                daylightOffset: (int)daylightOffset
            );

            timeZoneList.Add(timeZoneInfo);
            timeZoneList = timeZoneList.Where(tz => activeZones.Any(az => az.ZoneId == tz.Id)).ToList();


            foreach (var activeZoneSub in activeZones)
            {
                var matchingTimeZone = timeZoneList.FirstOrDefault(c => c.Id == activeZoneSub.ZoneId);

                if (matchingTimeZone != null)
                {
                    activeZoneSub.Offset = matchingTimeZone.Offset;
                    activeZoneSub.DaylightOffset = matchingTimeZone.DaylightOffset;
                }
            }
           
        }
        return activeZones;
    }
    private class FSTimeZoneInfo
    {
        public string Id { get; set; }
        public int Offset { get; set; }
        public int DaylightOffset { get; set; }

        public FSTimeZoneInfo(string id, int offset, int daylightOffset)
        {
            Id = id;
            Offset = offset;
            DaylightOffset = daylightOffset;
        }
    }
}
