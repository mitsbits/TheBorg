using System;

namespace Borg.Infra.EF6.Exceptions
{
    public class InvalidSequenceEntityException : Exception
    {
        public InvalidSequenceEntityException(Type type) : base($"{type.FullName} is invalid as a Sequence Entity")
        {
        }
    }
}