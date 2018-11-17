namespace Compiler.Core
{
    [System.Serializable]
    class TAssign
    {
        internal TVar Var { get; set; }
        internal TExpression Exp { get; set; }
        internal TExpression index { get; set; }
        internal TypeSymbol UL { get; set; }
    }
}
