namespace Compiler.Core
{
    [System.Serializable]
    class ReadInstruction
    {
        internal TVar V { get; set; }
        internal TExpression index { get; set; }
    }
}
