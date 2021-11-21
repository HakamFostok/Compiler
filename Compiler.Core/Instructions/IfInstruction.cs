namespace Compiler.Core;

[System.Serializable]
class IfInstruction
{
    internal BaseInstruction Ins { get; set; }
    internal TExpression Cond { get; set; }
    internal BaseInstruction InsElse { get; set; }
}
