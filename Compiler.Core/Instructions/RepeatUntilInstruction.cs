namespace Compiler.Core;

[System.Serializable]
class RepeatUntilInstruction
{
    internal TExpression Cond { get; set; }
    internal BaseInstruction Ins { get; set; }
}
