using System;

namespace Borg.Infra.Relational
{
    public class IncorrectUnitOfWorkUsageException : InvalidOperationException
    {
        public IncorrectUnitOfWorkUsageException()
        {
        }

        public IncorrectUnitOfWorkUsageException(string message)
            : base(message)
        {
        }

        public IncorrectUnitOfWorkUsageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}