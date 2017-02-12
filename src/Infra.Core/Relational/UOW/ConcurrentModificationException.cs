﻿using System;
using System.Runtime.Serialization;

namespace Borg.Infra.Relational
{
    [Serializable]
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException()
        {
        }

        public ConcurrentModificationException(string message)
            : base(message)
        {
        }

        public ConcurrentModificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConcurrentModificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}