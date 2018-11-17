namespace Compiler.Core
{
    [System.Serializable]
    class TRead
    {
        internal TVar V { get; set; }
        internal TExpression index { get; set; }
    }
}
