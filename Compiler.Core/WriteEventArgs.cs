using System;

namespace Compiler.Core
{
    public class WriteEventArgs : EventArgs
    {
        public bool IsLine { get; set; }
        public string Line { get; set; }
    }
}
