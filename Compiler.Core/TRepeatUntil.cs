namespace Compiler.Core
{
#if doc
    [System.Serializable]
    class TRepeatUntil
    {
        internal TExpression Cond { get; set; }
        internal TInstruction Ins { get; set; }
    }

#else
    
    class TRepeatUntil : TCondition
    {
    }

#endif
}