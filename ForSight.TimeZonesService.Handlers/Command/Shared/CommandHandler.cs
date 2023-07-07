using ForSight.TimeZonesService.Data;

namespace ForSight.TimeZonesService.Handlers.Command.Shared
{
    public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand
    {
        protected readonly ITimeZonesServiceDbContext TimeZonesServiceDbContext;

        protected CommandHandler(ITimeZonesServiceDbContext timeZonesServiceDbContext)
        {
            TimeZonesServiceDbContext = timeZonesServiceDbContext;
        }

        public Task<TResult> Execute(TCommand command)
        {
            return Handle(command);
        }

        protected abstract Task<TResult> Handle(TCommand command);
    }

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        protected readonly ITimeZonesServiceDbContext TimeZonesServiceDbContext;

        protected CommandHandler(ITimeZonesServiceDbContext timeZonesServiceDbContext)
        {
            TimeZonesServiceDbContext = timeZonesServiceDbContext;
        }

        public Task Execute(TCommand command)
        {
            return Handle(command);
        }

        protected abstract Task Handle(TCommand command);
    }
}
