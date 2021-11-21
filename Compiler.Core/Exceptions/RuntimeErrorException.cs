using System.Runtime.Serialization;

namespace Compiler.Core;

[Serializable]
public class RuntimeErrorException : ErrorBaseException
{
    public RuntimeErrorException() { }

    public RuntimeErrorException(string message)
        : base(message)
    { }

    public RuntimeErrorException(string message, Exception inner)
        : base(message, inner)
    { }

    protected RuntimeErrorException(
      SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}
