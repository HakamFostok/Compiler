using System.Runtime.Serialization;

namespace Compiler.Core;

[Serializable]
public class SyntaxErrorException : CompileTimeErrorException
{
    public SyntaxErrorException()
    { }

    public SyntaxErrorException(string message, int column, int lineNumber, string fileName)
        : base(message, column, lineNumber, fileName)
    { }

    public SyntaxErrorException(string message)
        : base(message)
    { }

    public SyntaxErrorException(string message, Exception inner)
        : base(message, inner)
    { }

    protected SyntaxErrorException(
      SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}
