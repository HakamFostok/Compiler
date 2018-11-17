namespace Compiler.Core
{
    internal class TSymbol
    {
        internal string Name { get; set; }
        internal TypeSymbol UL { get; set; }
        internal TSymbol Next { get; set; }
        
        internal static TSymbol FindSymbol(string name, TSymbol gSymbol)
        {
            TSymbol temp = gSymbol;
            while (temp != null)
            {
                if (temp.Name.ToLower() == name.ToLower())
                {
                    return temp;
                }
                temp = temp.Next;
            }

            return null;
        }
    }
}