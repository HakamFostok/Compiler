using System;
using System.Runtime.Serialization;

namespace Compiler.Core
{
    [Serializable]
    public class WarningException : CompileTimeErrorException
    {
        public WarningException()
        { }

        public WarningException(string message, int column, int lineNumber, string fileName)
            : base(message, column, lineNumber, fileName)
        { }

        public WarningException(string message)
            : base(message)
        { }

        public WarningException(string message, System.Exception inner)
            : base(message, inner)
        { }

        protected WarningException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
