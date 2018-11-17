namespace Compiler.Core
{
    [System.Serializable]
    class TWhile
    {
        internal TExpression Cond { get; set; }
        internal TInstruction Ins { get; set; }
    }
}