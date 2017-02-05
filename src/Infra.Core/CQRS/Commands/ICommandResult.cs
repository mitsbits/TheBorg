namespace Borg.Infra.CQRS
{
    public interface ICommandResult<out TEntity> : ICommandResult
    {
        TEntity Entity { get; }
    }

    public interface ICommandResult : IResponse
    {
        bool Succeded { get; }
        string Description { get; }
    }
}