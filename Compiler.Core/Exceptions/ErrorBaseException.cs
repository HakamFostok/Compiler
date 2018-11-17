using System;
using System.Runtime.Serialization;

namespace Compiler.Core
{
    [Serializable]
    public abstract class ErrorBaseException : ApplicationException
    {
        protected ErrorBaseException()
        { }

        protected ErrorBaseException(string message)
            : base(message)
        { }

        protected ErrorBaseException(string message, Exception inner) :
            base(message, inner)
        { }

        protected ErrorBaseException(
          SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}