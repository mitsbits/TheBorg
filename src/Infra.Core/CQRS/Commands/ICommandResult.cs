namespace Borg.Infra.CQRS
{
    public interface ICommandResult<out TEntity> : ICommandResult
    {
        TEntity Entity { get; }
    }

    public interface ICommandResult : IResponse
    {
        bool Success { get; }
        string Description { get; }
    }
}