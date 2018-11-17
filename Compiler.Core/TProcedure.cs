namespace Compiler.Core
{
    [System.Serializable]
    class TProcedure : TIdentifier
    {
        internal TVar PIN { get; set; }
        internal TVar POut { get; set; }
        internal TVar LVar { get; set; }
        internal TInstruction Linst { get; set; }
        internal bool IsDefined { get; set; }
        internal bool IsFunc { get; set; }
    }
}
