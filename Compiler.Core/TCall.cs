namespace Compiler.Core
{
    [System.Serializable]
    class TCall
    {
        internal TProcedure P { get; set; }
        internal TExpression Pin { get; set; }
        internal TListVar Pout { get; set; }
    }
}
