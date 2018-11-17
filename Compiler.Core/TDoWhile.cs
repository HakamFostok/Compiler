namespace Compiler.Core
{
    [System.Serializable]
    class TDoWhile
    {
        internal TExpression Cond { get; set; }
        internal TInstruction Ins { get; set; }
    }
}
