using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForSight.TimeZonesService.Data;

namespace ForSight.TimeZonesService.Handlers.Query.Shared
{
    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery
    {
        protected readonly ITimeZonesServiceDbContext TimeZonesServiceDbContext;

        protected QueryHandler(ITimeZonesServiceDbContext timeZonesServiceDbContext)
        {
            TimeZonesServiceDbContext = timeZonesServiceDbContext;
        }

        public Task<TResult> Get(TQuery query)
        {
            return Handle(query);
        }

        protected abstract Task<TResult> Handle(TQuery query);
    }    
}
