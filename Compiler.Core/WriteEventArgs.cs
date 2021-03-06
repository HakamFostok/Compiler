﻿using System;

namespace Compiler.Core
{
    public class ShowErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public string Column { get; }
        public string Line { get; }
        public string File { get; }
        public string ErrorType { get; }

        public ShowErrorEventArgs(string message, string column, string line, string file, string errorType)
        {
            this.Message = message;
            this.Column = column;
            this.Line = line;
            this.File = file;
            this.ErrorType = errorType;
        }
    }

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
