using System;

namespace Borg.Infra.CQRS
{
    public class NoRetrieversForQueryException : Exception
    {
        public NoRetrieversForQueryException(Type query) : base($"No retrievers are registered for {query.FullName}")
        {
        }
    }
}