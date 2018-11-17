namespace Compiler.Core
{
    [System.Serializable]
    public class TInstruction
    {
        internal object Ins { get; set; }
        internal TInstruction Next { get; set; }
    }
}
