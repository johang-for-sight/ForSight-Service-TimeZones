namespace ForSight.TimeZonesService.Handlers.Command.Shared
{
    public interface ICommandHandler<in TCommand>
    {
        Task Execute(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> Execute(TCommand command);
    }
}
