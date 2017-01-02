using System;

namespace Borg.Infra.CQRS
{
    public class MultipleRetrieversForQueryException : Exception
    {
        public MultipleRetrieversForQueryException(Type query) : base($"Multiple retrivers are registered for {query.FullName}")
        {
        }
    }
}