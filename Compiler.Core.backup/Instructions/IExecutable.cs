namespace Compiler.Core
{
    internal interface IExecutable
    {
        void Execute(LexicalAnalyst lexicalAnalyst);
    }
}