using System.Threading.Tasks;
using ForSight.TimeZonesService.Handlers.Command.Shared;
using ForSight.TimeZonesService.Handlers.Query.Shared;

namespace ForSight.TimeZonesService.API.Dispatcher
{
    public interface IDispatcher
    {
        Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query) where TQuery : IQuery;

        Task<TResult> DispatchCommand<TCommand, TResult>(TCommand command) where TCommand : ICommand;

        Task DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
