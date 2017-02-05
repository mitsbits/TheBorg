using System;

namespace Borg.Infra.CQRS
{
    public class CommandResult : ICommandResult
    {
        protected CommandResult(bool succeded, string description)
        {
            Succeded = succeded;
            Description = description;
        }

        public virtual string Description
        {
            get;
        }

        public virtual bool Succeded
        {
            get;
        }

        public static ICommandResult Create(bool success, string description = "")
        {
            return new CommandResult(success, description);
        }

        public static ICommandResult Create(Exception exception)
        {
            return new CommandResult(false, exception.ToString());
        }

        public static ICommandResult Success(string description = "")
        {
            return new CommandResult(true, description);
        }
    }

    public class CommandResult<TEntity> : CommandResult, ICommandResult<TEntity>
    {
        protected CommandResult(bool success, string description, TEntity entity) : base(success, description)
        {
            Entity = entity;
        }

        public virtual TEntity Entity
        {
            get;
        }

        public static ICommandResult Create(bool success, TEntity entity, string description = "")
        {
            return new CommandResult<TEntity>(success, description, entity);
        }

        public static ICommandResult Success(TEntity entity, string description = "")
        {
            return new CommandResult<TEntity>(true, description, entity);
        }
    }
}