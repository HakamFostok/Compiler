namespace Compiler.Core;

[System.Serializable]
class DoWhileInstruction
{
    internal TExpression Cond { get; set; }
    internal BaseInstruction Ins { get; set; }
}
