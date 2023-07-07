using System.Threading.Tasks;

namespace ForSight.TimeZonesService.Handlers.Query.Shared
{
    public interface IQueryHandler<in TQuery, TResult>
    {
        Task<TResult> Get(TQuery query);
    }
}
