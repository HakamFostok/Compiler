namespace Compiler.Core
{
    [System.Serializable]
    class TWrite
    {
        internal TExpression exp { get; set; }
        internal bool isLn { get; set; }
    }
}
