namespace Compiler.Core
{
    [System.Serializable]
    class WhileInstruction
    {
        internal TExpression Cond { get; set; }
        internal BaseInstruction Ins { get; set; }
    }
}