namespace Compiler.Core
{
    [System.Serializable]
    class TFor
    {
        internal TInstruction Ins { get; set; }
        internal TVar V { get; set; }
        internal TExpression ExpBegin { get; set; }
        internal TExpression ExpEnd { get; set; }
        internal bool IsDown { get; set; }
        internal TExpression ExpStep { get; set; }
    }
}