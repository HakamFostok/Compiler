namespace Compiler.Core
{
    [System.Serializable]
    class TExpression
    {
        internal TExpression Next { get; set; }
        internal TExpression Prev { get; set; }
        internal TypeSymbol UL { get; set; }
        internal string ValStr { get; set; }
        internal double ValNB { get; set; }
        internal TVar ValVar { get; set; }
        internal CallInstruction ValCall { get; set; }
        internal TExpression Index { get; set; }

        internal static TExpression CopyExpression(TExpression exp)
        {
            TExpression fstExp = null;
            TExpression lastExp = null;

            while (exp != null)
            {
                TExpression expnew = new TExpression();
                expnew.UL = exp.UL;
                expnew.ValNB = exp.ValNB;
                expnew.ValStr = exp.ValStr;
                expnew.ValVar = exp.ValVar;
                expnew.ValCall = exp.ValCall;
                expnew.Index = exp.Index;

                if (fstExp == null)
                {
                    fstExp = expnew;
                }
                else
                {
                    lastExp.Next = expnew;
                }

                expnew.Prev = lastExp;
                lastExp = expnew;
                exp = exp.Next;
                //expnew.Next = null;
            }

            return fstExp;
        }
    }

}