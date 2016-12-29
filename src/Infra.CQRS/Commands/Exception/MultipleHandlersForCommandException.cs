using System;

namespace Borg.Infra.CQRS
{
    public class MultipleHandlersForCommandException : System.Exception
    {
        public MultipleHandlersForCommandException(Type command) : base($"Multiple handlers are registered for {command.FullName}")
        {
        }
    }
}