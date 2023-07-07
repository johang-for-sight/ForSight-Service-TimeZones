using System;
using System.Threading.Tasks;
using ForSight.TimeZonesService.Handlers.Command.Shared;
using ForSight.TimeZonesService.Handlers.Query.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace ForSight.TimeZonesService.API.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            using var scope = _serviceProvider.CreateScope();
            var queryHandler = scope.ServiceProvider.GetService<IQueryHandler<TQuery, TResult>>();

            if (queryHandler == null)
            {
                throw new TypeLoadException($"QueryHandler not found for type: {nameof(query)}");
            }

            return await queryHandler.Get(query);
        }

        public async Task<TResult> DispatchCommand<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            using var scope = _serviceProvider.CreateScope();
            var commandHandler = scope.ServiceProvider.GetService<ICommandHandler<TCommand, TResult>>();

            if (commandHandler == null)
            {
                throw new TypeLoadException($"CommandHandler not found for type: {nameof(command)}");
            }

            return await commandHandler.Execute(command);
        }

        public async Task DispatchCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            using var scope = _serviceProvider.CreateScope();
            var commandHandler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>();

            if (commandHandler == null)
            {
                throw new TypeLoadException($"CommandHandler not found for type: {nameof(command)}");
            }

            await commandHandler.Execute(command);
        }        
    }
}
