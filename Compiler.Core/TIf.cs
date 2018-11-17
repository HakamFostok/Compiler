namespace Compiler.Core
{
    [System.Serializable]
    class TIf 
    {
        internal TInstruction Ins { get; set; }
        internal TExpression Cond { get; set; }
        internal TInstruction InsElse { get; set; }
    }
}