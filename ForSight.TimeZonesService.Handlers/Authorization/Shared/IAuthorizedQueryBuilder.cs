using System.Linq;
using ForSight.TimeZonesService.Data;

namespace ForSight.TimeZonesService.Handlers.Authorization.Shared
{
    public interface IAuthorizedQueryBuilder<out TEntity> where TEntity : class
    {
        IQueryable<TEntity> ComposeQuery(IRequestContext<ITimeZonesServiceDbContext> requestContext);
    }
}