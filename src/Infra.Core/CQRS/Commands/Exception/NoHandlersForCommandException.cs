using System;

namespace Borg.Infra.CQRS
{
    public class NoHandlersForCommandException : Exception
    {
        public NoHandlersForCommandException(Type command) : base($"No handlers are registered for {command.FullName}")
        {
        }
    }
}