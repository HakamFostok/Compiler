using System.Runtime.Serialization;

namespace Compiler.Core;

[Serializable]
public abstract class CompileTimeErrorException : ErrorBaseException
{
    public string FileName { get; private set; }
    public int LineNumber { get; private set; }
    public int Column { get; private set; }

    protected CompileTimeErrorException(string message, int column, int lineNumber, string fileName)
        : base(message)
    {
        FileName = fileName;
        LineNumber = lineNumber;
        Column = column;
    }

    protected CompileTimeErrorException()
    { }

    protected CompileTimeErrorException(string message)
        : base(message)
    { }

    protected CompileTimeErrorException(string message, Exception inner) :
        base(message, inner)
    { }

    protected CompileTimeErrorException(
      SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}
