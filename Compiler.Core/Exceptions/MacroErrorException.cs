using System;
using System.Runtime.Serialization;

namespace Compiler.Core
{
    [Serializable]
    public class MacroErrorException : CompileTimeErrorException
    {
        public MacroErrorException()
        { }
         
        public MacroErrorException(string message, int column, int lineNumber, string fileName)
            : base(message, column, lineNumber, fileName)
        { }

        public MacroErrorException(string message)
            : base(message)
        { }

        public MacroErrorException(string message, Exception inner)
            : base(message, inner)
        { }

        protected MacroErrorException(
          SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
