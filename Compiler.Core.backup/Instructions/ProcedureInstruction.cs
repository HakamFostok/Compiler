namespace Compiler.Core
{
    [System.Serializable]
    class ProcedureInstruction : IdentifierInstruction
    {
        internal TVar PIN { get; set; }
        internal TVar POut { get; set; }
        internal TVar LVar { get; set; }
        internal BaseInstruction Linst { get; set; }
        internal bool IsDefined { get; set; }
        internal bool IsFunc { get; set; }
    }
}
