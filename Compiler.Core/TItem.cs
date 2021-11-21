namespace Compiler.Core;

[System.Serializable]
internal class TItem
{
    internal TItem Next { get; set; }
    internal TypeSymbol UL { get; set; }
    internal double ValNB { get; set; }
    internal string ValStr { get; set; }
}
