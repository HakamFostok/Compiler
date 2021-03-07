namespace Compiler.Core
{
    [System.Serializable]
    class TListVar
    {
        internal TVar V { get; set; }
        internal TListVar Next { get; set; }
    }
}
