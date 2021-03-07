namespace Compiler.Core
{
#if doc
    [System.Serializable]
    class RepeatUntilInstruction
    {
        internal TExpression Cond { get; set; }
        internal BaseInstruction Ins { get; set; }
    }

#else
    
    class TRepeatUntil : TCondition
    {
    }

#endif
}