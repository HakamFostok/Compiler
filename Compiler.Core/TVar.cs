namespace Compiler.Core;

[System.Serializable]
class TVar : IdentifierInstruction
{
    internal TItem items { get; set; }
}
