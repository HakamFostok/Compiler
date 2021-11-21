namespace Compiler.Core;

[System.Serializable]
class CallInstruction
{
    internal ProcedureInstruction P { get; set; }
    internal TExpression Pin { get; set; }
    internal TListVar Pout { get; set; }
}
