namespace Compiler.Core;

[System.Serializable]
public class BaseInstruction
{
    internal object Ins { get; set; }
    internal BaseInstruction Next { get; set; }
}
