using System;

namespace Compiler.Core
{
    public class WriteEventArgs : EventArgs
    {
        public bool IsLine { get; }
        public string Line { get; }
        public bool EndOfExecution { get; }

        public WriteEventArgs(bool isLine, string line, bool endOfExecution)
        {
            IsLine = isLine;
            Line = line;
            EndOfExecution = endOfExecution;
        }
    }
}
