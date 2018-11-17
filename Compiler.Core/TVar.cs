namespace Compiler.Core
{
    [System.Serializable]
    class TVar : TIdentifier
    {
        internal TItem items { get; set; }
    }
}