using System.Runtime.Serialization;

namespace Compiler.Core;

[Serializable]
public class LexicalErrorException : CompileTimeErrorException
{
    public LexicalErrorException()
    { }

    public LexicalErrorException(string message, int column, int lineNumber, string fileName)
        : base(message, column, lineNumber, fileName)
    { }

    public LexicalErrorException(string message)
        : base(message)
    { }

    public LexicalErrorException(string message, Exception inner)
        : base(message, inner)
    { }

    protected LexicalErrorException(
      SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}
