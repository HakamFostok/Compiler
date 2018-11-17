namespace Compiler.Core
{
    [System.Serializable]
    class ForInstruction
    {
        internal BaseInstruction Ins { get; set; }
        internal TVar V { get; set; }
        internal TExpression ExpBegin { get; set; }
        internal TExpression ExpEnd { get; set; }
        internal bool IsDown { get; set; }
        internal TExpression ExpStep { get; set; }
    }
}