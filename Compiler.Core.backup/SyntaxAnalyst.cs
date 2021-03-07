using System.Collections.Generic;
using System.Linq;

namespace Compiler.Core
{
    partial class SyntaxAnalyst
    {
        private Variables Locals { get; set; }
        private LexicalAnalyst lexer { get; set; }
        private TypeSymbol UL { get; set; }
        private List<WarningException> warnning { get; set; }

        private ProcedureInstruction gProc
        {
            get
            {
                return Locals.GPorc;
            }
        }

        private ProcedureInstruction currProc
        {
            get
            {
                return Locals.CurrPorc;
            }
            set
            {
                Locals.CurrPorc = value;
            }
        }

        private string G_curr_Str
        {
            get
            {
                return Locals.G_curr_str;
            }
        }

        private IdentifierInstruction G_curr_ID
        {
            get
            {
                return Locals.G_curr_id;
            }
            set
            {
                Locals.G_curr_id = value;
            }
        }

        private TVar gVar
        {
            get
            {
                return Locals.gvar;
            }
            set
            {
                Locals.gvar = value;
            }
        }

        private int CI
        {
            get
            {
                return Locals.CI;
            }
        }

        private double G_curr_Num
        {
            get
            {
                return Locals.G_curr_NB;
            }
        }

        private int lineNumber
        {
            get
            {
                return Locals.lineNumber;
            }
        }

        private TypeSymbol LexicalUnit()
        {
            return lexer.LexicalUnit();
        }

        public SyntaxAnalyst(Variables loc, List<WarningException> warn)
        {
            lexer = new LexicalAnalyst(loc);
            warnning = warn;
            Locals = loc;
        }

        private void GetSyntaxError(string message, params string[] parameters)
        {
            MakeSyntaxError(string.Format(message, parameters));
        }

        //maybe this method is not necessary.
        private void GetSyntaxError(string message)
        {
            MakeSyntaxError(message);
        }

        private void MakeSyntaxError(string fullMessage)
        {
            throw new SyntaxErrorException(fullMessage, CI, lineNumber, Locals.CurrFile.Name);
        }

        internal void CompileCurrentFile()
        {
            lexer.PreProcess();
            ReadProcedures();
        }

        private CallInstruction ReadCall(ProcedureInstruction procAux)
        {
            CallInstruction callAux = new CallInstruction { P = procAux };

            // Code before Modify

            //callAux.P = (TProcedure)lexer.CurrID;
            //if (LexicalUnit() != TypeSymbol.U_OpenParanthese)
            //{
            //    GetSyntaxError(WordMessagesError.NotFound, "(");
            //}

            // Code after Modify
            // callAux.P = procAux;
            if (UL != TypeSymbol.U_OpenParanthese)
            {
                GetSyntaxError(WordMessagesError.NotFound, "(");
            }

            UL = LexicalUnit();
            // By Default
            callAux.Pin = null;
            callAux.Pout = null;

            if (UL == TypeSymbol.U_Input)
            {
                UL = LexicalUnit();
                TExpression last = null;
                while (true)
                {
                    TExpression last1 = null;
                    TExpression exp1 = ReadExpression(ref last1);
                    if (callAux.Pin == null)
                    {
                        callAux.Pin = exp1;
                    }
                    else
                    {
                        last.Next = exp1;
                        exp1.Prev = last;
                    }

                    last = last1;

                    if (UL == TypeSymbol.U_ClosedParanthese || UL == TypeSymbol.U_Output)
                    {
                        break;
                    }
                    if (UL != TypeSymbol.U_Comma)
                    {
                        GetSyntaxError(WordMessagesError.NotFound, "\",\"");
                    }

                    UL = LexicalUnit();
                }   // end while
            }

            if (UL == TypeSymbol.U_Output) // this section must be Repated again ( it is not finished )
            {
                UL = LexicalUnit();

                while (true)
                {
                    if (UL == TypeSymbol.U_UnKown)
                    {
                        IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                        G_curr_ID = gVar;
                    }
                    else if (UL != TypeSymbol.U_Var)
                    {
                        GetSyntaxError(WordMessagesError.NotFound, "Variable");
                    }

                    TListVar newlvar = new TListVar();
                    newlvar.V = (TVar)G_curr_ID;
                    newlvar.Next = callAux.Pout;
                    callAux.Pout = newlvar;

                    UL = LexicalUnit();
                    if (UL == TypeSymbol.U_ClosedParanthese)
                    {
                        break;
                    }
                    if (UL != TypeSymbol.U_Comma)
                    {
                        GetSyntaxError(WordMessagesError.NotFound, "\",\"");
                    }
                    UL = LexicalUnit();
                }
            }

            if (UL != TypeSymbol.U_ClosedParanthese)
            {
                GetSyntaxError(WordMessagesError.NotFound, ")");
            }

            UL = LexicalUnit();
            return callAux;
        }

        private TExpression ReadCondition()
        {
            TExpression last = null;
            return ReadCondition(ref last);
        }

        private TExpression ReadCondition(ref TExpression last)
        {
            TExpression exp0 = ReadCTerm(ref last);

            while (UL == TypeSymbol.U_Or)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadCTerm(ref last1);

                last.Next = exp1;
                exp1.Prev = last;
                last1.Next = expNew;
                expNew.Prev = last1;
                expNew.Next = null;
                last = expNew;
            }

            return exp0;
        }

        private TExpression ReadCTerm(ref TExpression last)
        {
            TExpression exp0 = ReadCFactor(ref last);

            while (UL == TypeSymbol.U_And)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadCFactor(ref last1);

                last.Next = exp1;
                exp1.Prev = last;
                last1.Next = expNew;
                expNew.Prev = last1;
                expNew.Next = null;
                last = expNew;
            }

            return exp0;

        }

        private TExpression ReadCFactor(ref TExpression last)
        {
            TExpression exp0 = ReadExpression(ref last);

            while (UL == TypeSymbol.U_LessEqual || UL == TypeSymbol.U_LessThan ||
                UL == TypeSymbol.U_GreaterEqual || UL == TypeSymbol.U_GreaterThan ||
                UL == TypeSymbol.U_Equal || UL == TypeSymbol.U_NotEqual)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadExpression(ref last1);

                last.Next = exp1;
                exp1.Prev = last;
                last1.Next = expNew;
                expNew.Prev = last1;
                expNew.Next = null;
                last = expNew;
            }

            return exp0;
        }

        private TExpression ReadExpression()
        {
            TExpression last = null;
            return ReadExpression(ref last);
        }

        private TExpression ReadExpression(ref TExpression last)
        {
            TExpression exp0 = ReadTerm(ref last);
            while (UL == TypeSymbol.U_Pluse || UL == TypeSymbol.U_Minus)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadTerm(ref last1);

                last.Next = exp1;
                exp1.Prev = last;
                last1.Next = expNew;
                expNew.Prev = last1;
                expNew.Next = null; // by default
                last = expNew;
            }

            return exp0;
        }

        private TExpression ReadTerm(ref TExpression last)
        {
            TExpression exp0 = ReadFactor(ref last);

            while (UL == TypeSymbol.U_IntDiv || UL == TypeSymbol.U_Multiply ||
                UL == TypeSymbol.U_Division || UL == TypeSymbol.U_Mod)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadFactor(ref last1);

                last.Next = exp1;
                exp1.Prev = last;
                last1.Next = expNew;
                expNew.Prev = last1;
                expNew.Next = null;
                last = expNew;
            }

            return exp0;
        }

        private TExpression ReadFactor(ref TExpression last)
        {
            TExpression exp0 = ReadFact(ref last);
            TExpression lastAux = last;
            while (UL == TypeSymbol.U_Pow)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                TExpression last1 = null;
                TExpression exp1 = ReadFact(ref last1);
                expNew.Next = lastAux.Next;
                if (lastAux.Next != null)
                {
                    lastAux.Next.Prev = expNew;
                }
                else
                {
                    last = expNew;
                }
                lastAux.Next = exp1;
                exp1.Prev = lastAux;
                last1.Next = expNew;
                expNew.Prev = last1;
                lastAux = last1;
            }
            return exp0;
        }

        private TExpression ReadFact(ref TExpression last)
        {
            if (UL == TypeSymbol.U_Cst_Int || UL == TypeSymbol.U_Cst_Real || UL == TypeSymbol.U_Cst_Str ||
                UL == TypeSymbol.U_True || UL == TypeSymbol.U_False)
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                if (UL == TypeSymbol.U_Cst_Str)
                {
                    expNew.ValStr = G_curr_Str;
                }
                else if (UL == TypeSymbol.U_Cst_Int || UL == TypeSymbol.U_Cst_Real) // the if statment i added after notce the true false.
                {
                    expNew.ValNB = G_curr_Num;
                }

                // by default
                expNew.Next = null;
                expNew.Prev = null;

                last = expNew;
                UL = LexicalUnit();
                return expNew;
            }
            else if (UL == TypeSymbol.U_OpenParanthese)
            {
                UL = LexicalUnit();
                TExpression expNew = ReadCondition(ref last);
                if (UL != TypeSymbol.U_ClosedParanthese)
                {
                    GetSyntaxError(WordMessagesError.NotFound, ")");
                }
                UL = LexicalUnit();
                return expNew;
            }
            else if (UL == TypeSymbol.U_Var || UL == TypeSymbol.U_VarDefine ||
                UL == TypeSymbol.U_VarProcedure || UL == TypeSymbol.U_UnKown)
            {
                TExpression expNew = new TExpression();

                // By Default
                expNew.Next = null;
                expNew.Prev = null;

                TVar varAux = null;
                ProcedureInstruction procAux = null;
                TypeSymbol ulAux;

                if (UL == TypeSymbol.U_VarDefine)   // if the variable define
                {
                    DefineInstruction defAux = (DefineInstruction)G_curr_ID;
                    expNew.UL = defAux.UL;
                    expNew.ValNB = defAux.ValNB;
                    expNew.ValStr = defAux.ValStr;
                    ulAux = defAux.UL;
                    UL = LexicalUnit();
                }
                else if (UL == TypeSymbol.U_UnKown) // if the variblae unknown procedure , unknown variable
                {
                    string buffer = G_curr_Str;
                    UL = LexicalUnit();

                    if (UL == TypeSymbol.U_OpenParanthese)  // if the variblae unknown procedure
                    {
                        IdentifierInstruction.AddIdentifier(buffer, ref Locals.gproc);
                        procAux = gProc;
                        gProc.IsFunc = true;
                        //gProc.IsDefined = false;
                        //gProc.PIN = null;
                        //gProc.POut = null;
                        //gproc.LVar = null;
                        //gProc.Linst = null;
                        ulAux = TypeSymbol.U_VarProcedure;

                    }
                    else                    // if the variblae unknown procedure , unknownvariable
                    {
                        IdentifierInstruction.AddIdentifier(buffer, ref Locals.gvar);
                        varAux = gVar;
                        ulAux = TypeSymbol.U_Var;
                    }
                }
                else    // if the variable known
                {
                    // this statment i am not sure about it
                    ulAux = UL;
                    if (ulAux == TypeSymbol.U_Var)      // if the vairable known var
                    {
                        varAux = (TVar)G_curr_ID;
                    }
                    else                                // if the variable known procedure
                    {
                        procAux = (ProcedureInstruction)G_curr_ID;
                    }

                    UL = LexicalUnit();
                }

                expNew.UL = ulAux;
                last = expNew;
                if (ulAux == TypeSymbol.U_VarProcedure)
                {
                    expNew.ValCall = ReadCall(procAux);
                }
                else if (ulAux == TypeSymbol.U_Var)
                {
                    expNew.ValVar = varAux;
                    if (UL == TypeSymbol.U_OpenBracket)
                    {
                        UL = LexicalUnit();
                        expNew.Index = ReadExpression();
                        if (UL != TypeSymbol.U_ClosedBracket)
                        {
                            GetSyntaxError(WordMessagesError.NotFound, "]");
                        }

                        UL = LexicalUnit();
                    }
                    else
                    {
                        expNew.Index = null;
                    }
                }

                return expNew;
            }
            else if (new TypeSymbol[] { TypeSymbol.U_Sin, TypeSymbol.U_Cos, TypeSymbol.U_Tg,
                TypeSymbol.U_Ln,TypeSymbol.U_Log, TypeSymbol.U_Exp, TypeSymbol.U_Sqr, TypeSymbol.U_Sqrt,
                TypeSymbol.U_Length, TypeSymbol.U_IntToStr, TypeSymbol.U_StrToInt, TypeSymbol.U_IntToHex,
                }.Contains(UL))
            {
                TExpression expNew = new TExpression();
                expNew.UL = UL;
                UL = LexicalUnit();
                if (UL != TypeSymbol.U_OpenParanthese)
                {
                    GetSyntaxError(WordMessagesError.NotFound, "(");
                }

                UL = LexicalUnit();
                TExpression exp0 = ReadExpression(ref last);
                last.Next = expNew;
                expNew.Prev = last;
                last = expNew;

                if (UL != TypeSymbol.U_ClosedParanthese)
                {
                    GetSyntaxError(WordMessagesError.NotFound, ")");
                }

                UL = LexicalUnit();
                return exp0;
            }
            else if (UL == TypeSymbol.U_Pluse || UL == TypeSymbol.U_Minus || UL == TypeSymbol.U_Not || UL == TypeSymbol.U_Complement)
            {
                TExpression expNew = new TExpression();

                if (UL == TypeSymbol.U_Pluse)
                {
                    expNew.UL = TypeSymbol.U_UnaryPluse;
                }
                else if (UL == TypeSymbol.U_Minus)
                {
                    expNew.UL = TypeSymbol.U_UnaryMinuse;
                }
                else if (UL == TypeSymbol.U_Not)
                {
                    expNew.UL = TypeSymbol.U_Not;
                }
                else
                {
                    expNew.UL = TypeSymbol.U_Complement;
                }

                UL = LexicalUnit();
                TExpression exp0 = ReadFact(ref last);
                last.Next = expNew;
                expNew.Prev = last;
                last = expNew;  // this statment i added to solve the problem that unary pluse and unary minus.

                return exp0;
            }
            else
            {
                GetSyntaxError(SyntaxMessagesError.InvalidToken, UL.ToString());
            }

            // this statment will never execute 
            // just for elminate compile time error.
            return null;
        }

        private void ReadProcedures()
        {
            UL = LexicalUnit();

            while (UL == TypeSymbol.U_Function || UL == TypeSymbol.U_Procedure)
            {
                TypeSymbol auxUl = UL;
                UL = LexicalUnit();

                if (UL == TypeSymbol.U_UnKown)
                {
                    IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gproc);
                    currProc = gProc;
                    currProc.IsDefined = true;
                }
                else if (UL == TypeSymbol.U_VarProcedure)
                {
                    currProc = (ProcedureInstruction)G_curr_ID;
                    if (currProc.IsDefined)
                    {
                        GetSyntaxError(SyntaxMessagesError.DuplicateDefination, currProc.Name);
                    }
                    currProc.IsDefined = true;
                    // this want to check.
                    if (currProc.IsFunc != (auxUl == TypeSymbol.U_Function))
                    {
                        GetSyntaxError(SyntaxMessagesError.NotAsCalled, currProc.Name);
                    }
                }
                else
                {
                    GetSyntaxError(WordMessagesError.NotFound, "Name of method");
                }

                currProc.IsFunc = (auxUl == TypeSymbol.U_Function);
                UL = LexicalUnit();
                if (UL != TypeSymbol.U_OpenParanthese)
                {
                    GetSyntaxError(WordMessagesError.NotFound, "(");
                }

                UL = LexicalUnit();

                // we should be sure that  Gvar is null;
                if (UL == TypeSymbol.U_Input)
                {
                    UL = LexicalUnit();
                    while (true)
                    {
                        if (UL != TypeSymbol.U_UnKown)
                        {
                            GetSyntaxError(WordMessagesError.NotFound, "Undifined variable");
                        }
                        IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                        UL = LexicalUnit();
                        if (UL == TypeSymbol.U_Comma)
                        {
                            UL = LexicalUnit();
                        }
                        else
                        {
                            break;
                        }
                    }

                    currProc.PIN = gVar;
                }
                if (UL == TypeSymbol.U_Output)
                {
                    UL = LexicalUnit();
                    while (true)
                    {
                        if (UL != TypeSymbol.U_UnKown)
                        {
                            GetSyntaxError(WordMessagesError.NotFound, "Undifined variable");
                        }
                        IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                        UL = LexicalUnit();
                        if (UL == TypeSymbol.U_Comma)
                        {
                            UL = LexicalUnit();
                        }
                        else
                        {
                            break;
                        }
                    }
                    currProc.POut = gVar;
                }

                if (UL != TypeSymbol.U_ClosedParanthese)
                {
                    GetSyntaxError(WordMessagesError.NotFound, ")");
                }
                UL = LexicalUnit();
                if (UL != TypeSymbol.U_SemiColon)
                {
                    GetSyntaxError(WordMessagesError.NotFound, ";");
                }

                UL = LexicalUnit();
                currProc.Linst = ReadListOfInstruction();
                currProc.LVar = gVar;
                gVar = null;
            }   // end while

            if (UL != TypeSymbol.U_EOF)
            {
                GetSyntaxError(SyntaxMessagesError.InsideMethod);
            }
        }

        private BaseInstruction ReadOneOrListOfInstruction()
        {
            UL = LexicalUnit();
            if (UL == TypeSymbol.U_Begin || UL == TypeSymbol.U_OpenBraces)
            {
                return ReadListOfInstruction();
            }

            BaseInstruction newIns = new BaseInstruction();
            newIns.Ins = ReadOneInstruction();
            newIns.Next = null;  // by default
            return newIns;
        }

        private BaseInstruction ReadListOfInstruction()
        {
            if (UL != TypeSymbol.U_Begin && UL != TypeSymbol.U_OpenBraces)
            {
                GetSyntaxError(WordMessagesError.NotFound, "Begin or {");
            }

            UL = LexicalUnit();

            BaseInstruction firstInstuction = null;
            BaseInstruction lastInstruction = null;

            while ((UL != TypeSymbol.U_ClosedBraces) && (UL != TypeSymbol.U_End) && (UL != TypeSymbol.U_EOF))
            {
                BaseInstruction newInst = new BaseInstruction { Ins = ReadOneInstruction() };
                if (firstInstuction == null)
                {
                    firstInstuction = newInst;
                }
                else
                {
                    lastInstruction.Next = newInst;
                }

                lastInstruction = newInst;
            }

            UL = LexicalUnit();  // I added this statment;
            return firstInstuction;
        }

        private object ReadOneInstruction()
        {
            if (UL == TypeSymbol.U_Var || UL == TypeSymbol.U_UnKown)
            {
                return HandleAssign();
            }
            else if (UL == TypeSymbol.U_If)
            {
                return HandleIf();
            }
            else if (UL == TypeSymbol.U_While)
            {
                return HandleWhile();
            }
            else if (UL == TypeSymbol.U_For)
            {
                return HandleFor();
            }
            else if (UL == TypeSymbol.U_Repeat)
            {
                return HandleRepeatUntil();
            }
            else if (UL == TypeSymbol.U_Do)
            {
                return HandleDoWhile();
            }
            else if (UL == TypeSymbol.U_Call)
            {
                return HandleCall();
            }
            else if (UL == TypeSymbol.U_Write || UL == TypeSymbol.U_WriteLn)
            {
                return HandleWrite();
            }
            else if (UL == TypeSymbol.U_Read)
            {
                return HandleRead();
            }
            else if (UL == TypeSymbol.U_Return)
            {
                return HandleReturn();
            }
            else if (UL == TypeSymbol.U_Break || UL == TypeSymbol.U_Halt || UL == TypeSymbol.U_Exit)
            {
                return HandleBreadHaltExit();
            }
            else
            {
                GetSyntaxError(SyntaxMessagesError.InvalidToken, UL.ToString());
            }
            return null;
        }

        private object HandleAssign()
        {
            AssignInstruction assignAux = new AssignInstruction();
            if (UL == TypeSymbol.U_UnKown)
            {
                IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                G_curr_ID = gVar;
            }

            assignAux.Var = (TVar)G_curr_ID;
            UL = LexicalUnit();
            assignAux.index = null;

            if (UL == TypeSymbol.U_OpenBracket)
            {
                UL = LexicalUnit();
                assignAux.index = ReadExpression();
                if (UL != TypeSymbol.U_ClosedBracket)
                {
                    GetSyntaxError(WordMessagesError.NotFound, "]");
                }
                UL = LexicalUnit();
            }

            // validation.
            if (!(UL == TypeSymbol.U_Assignment || UL == TypeSymbol.U_PluseAssigment || UL == TypeSymbol.U_MinusAssigment ||
                UL == TypeSymbol.U_MultiplyAssigment || UL == TypeSymbol.U_PowAssigment || UL == TypeSymbol.U_DivisionAssigment ||
                UL == TypeSymbol.U_ModAssigment || UL == TypeSymbol.U_PlusePluse || UL == TypeSymbol.U_MinusMinus))
            {
                GetSyntaxError(WordMessagesError.NotFound, ":= , Compound Assignmet , ++ or -- ");
            }

            assignAux.UL = UL;
            UL = LexicalUnit();
            if (assignAux.UL != TypeSymbol.U_PlusePluse && assignAux.UL != TypeSymbol.U_MinusMinus)
            {

                assignAux.Exp = ReadExpression();
            }

            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return assignAux;
        }

        private object HandleIf()
        {
            IfInstruction ifAux = new IfInstruction();

            UL = LexicalUnit();
            ifAux.Cond = ReadCondition();
            if (UL != TypeSymbol.U_Then)
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"Then\" KeyWord");
            }
            ifAux.Ins = ReadOneOrListOfInstruction();
            ifAux.InsElse = null;   // be deufault
            if (UL == TypeSymbol.U_Else)
            {
                ifAux.InsElse = ReadOneOrListOfInstruction();
            }
            return ifAux;
        }

        private object HandleWhile()
        {
            WhileInstruction whileAux = new WhileInstruction();

            UL = LexicalUnit();
            whileAux.Cond = ReadCondition();
            if (UL != TypeSymbol.U_Do)
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"Do\" key work");
            }
            whileAux.Ins = ReadOneOrListOfInstruction();
            return whileAux;
        }

        private object HandleFor()
        {
            ForInstruction forAux = new ForInstruction();
            UL = LexicalUnit();
            if (UL == TypeSymbol.U_UnKown)
            {
                IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                G_curr_ID = gVar;
            }
            else if (UL != TypeSymbol.U_Var)
            {
                GetSyntaxError(WordMessagesError.NotFound, "Variable");
            }
            forAux.V = (TVar)G_curr_ID;

            UL = LexicalUnit();
            if (UL != TypeSymbol.U_Assignment)
            {
                GetSyntaxError(WordMessagesError.NotFound, ":=");
            }
            UL = LexicalUnit();
            forAux.ExpBegin = ReadExpression();

            if ((UL != TypeSymbol.U_To) && (UL != TypeSymbol.U_DownTo))
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"To\" or \"DownTo\" Keyword");
            }
            forAux.IsDown = (UL == TypeSymbol.U_DownTo);

            UL = LexicalUnit();
            forAux.ExpEnd = ReadExpression();
            if (UL == TypeSymbol.U_Step)
            {
                UL = LexicalUnit();
                forAux.ExpStep = ReadExpression();
            }
            else
            {
                forAux.ExpStep = new TExpression { ValNB = 1, UL = TypeSymbol.U_Cst_Int };
            }

            if (UL != TypeSymbol.U_Do)
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"Do\" Keyword");
            }

            forAux.Ins = ReadOneOrListOfInstruction();
            return forAux;
        }

        private object HandleRepeatUntil()
        {
            RepeatUntilInstruction repeatAux = new RepeatUntilInstruction();
            repeatAux.Ins = ReadOneOrListOfInstruction();
            if (UL != TypeSymbol.U_Until)
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"Until\" keyword");
            }

            UL = LexicalUnit();
            repeatAux.Cond = ReadCondition();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return repeatAux;
        }

        private object HandleDoWhile()
        {
            DoWhileInstruction doWhileAux = new DoWhileInstruction();
            doWhileAux.Ins = ReadOneOrListOfInstruction();
            if (UL != TypeSymbol.U_While)
            {
                GetSyntaxError(WordMessagesError.NotFound, "\"while\" keyword");
            }

            UL = LexicalUnit();
            doWhileAux.Cond = ReadCondition();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return doWhileAux;
        }

        private object HandleCall()
        {
            UL = LexicalUnit();
            if ((UL != TypeSymbol.U_VarProcedure) && (UL != TypeSymbol.U_UnKown))
            {
                GetSyntaxError(WordMessagesError.NotFound, "Procedure Name");
            }
            if (UL == TypeSymbol.U_UnKown)
            {
                IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gproc);
                // by default
                gProc.IsDefined = false;
                gProc.IsFunc = false;
                gProc.Linst = null;
                gProc.PIN = null;
                gProc.POut = null;
                gProc.LVar = null;
                G_curr_ID = gProc;
            }
            if (((ProcedureInstruction)G_curr_ID).IsFunc)
            {
                GetSyntaxError(SyntaxMessagesError.CallFunction);
            }

            ProcedureInstruction procAux = (ProcedureInstruction)G_curr_ID;
            UL = LexicalUnit();
            CallInstruction callAux = ReadCall(procAux);

            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return callAux;
        }

        private object HandleWrite()
        {
            TWrite writeAux = new TWrite();
            writeAux.isLn = (UL == TypeSymbol.U_WriteLn);
            if (LexicalUnit() != TypeSymbol.U_OpenParanthese)
            {
                GetSyntaxError(WordMessagesError.NotFound, "(");
            }

            UL = LexicalUnit();

            // by default
            writeAux.exp = null;

            TExpression last = null;
            while (UL != TypeSymbol.U_ClosedParanthese)
            {
                TExpression last0 = null;
                TExpression exp0 = ReadExpression(ref last0);

                if (writeAux.exp == null)
                {
                    writeAux.exp = exp0;
                }
                else
                {
                    last.Next = exp0;
                    exp0.Prev = last;
                }

                last = last0;

                if (UL != TypeSymbol.U_ClosedParanthese && UL != TypeSymbol.U_Comma)
                {
                    GetSyntaxError(WordMessagesError.NotFound, ", or )");
                }

                if (UL == TypeSymbol.U_Comma)
                {
                    UL = LexicalUnit();
                    if (UL == TypeSymbol.U_ClosedParanthese)
                    {
                        GetSyntaxError(SyntaxMessagesError.ExpMiss, "\",\"", "\")\"");
                    }
                }
            }   // end while

            if (!writeAux.isLn && writeAux.exp == null)
            {
                warnning.Add(new WarningException(SyntaxMessagesError.NoOperation, CI, lineNumber, Locals.CurrFile.Name));
            }
            UL = LexicalUnit();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return writeAux;
        }

        private object HandleRead()
        {
            ReadInstruction readAux = new ReadInstruction();

            if (LexicalUnit() != TypeSymbol.U_OpenParanthese)
            {
                GetSyntaxError(WordMessagesError.NotFound, "(");
            }

            UL = LexicalUnit();
            if (UL == TypeSymbol.U_UnKown)
            {
                IdentifierInstruction.AddIdentifier(G_curr_Str, ref Locals.gvar);
                readAux.V = gVar;
            }
            else if (UL == TypeSymbol.U_Var)
            {
                readAux.V = (TVar)G_curr_ID;
            }
            else
            {
                GetSyntaxError(WordMessagesError.NotFound, "Variable");
            }

            UL = LexicalUnit();
            if (UL == TypeSymbol.U_OpenBracket)
            {
                UL = LexicalUnit();
                readAux.index = ReadExpression();

                if (UL != TypeSymbol.U_ClosedBracket)
                {
                    GetSyntaxError(WordMessagesError.NotFound, "]");
                }

                UL = LexicalUnit();
            }

            if (UL != TypeSymbol.U_ClosedParanthese)
            {
                GetSyntaxError(WordMessagesError.NotFound, ")");
            }

            UL = LexicalUnit();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return readAux;
        }

        private object HandleReturn()
        {
            ReturnInstruction returnAux = new ReturnInstruction();
            UL = LexicalUnit();
            returnAux.Exp = ReadExpression();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return returnAux;
        }

        private object HandleBreadHaltExit()
        {
            BreakInstruction breakAux = new BreakInstruction();
            breakAux.UL = UL;
            UL = LexicalUnit();
            if (UL != TypeSymbol.U_SemiColon)
            {
                GetSyntaxError(SyntaxMessagesError.SemiColon);
            }
            UL = LexicalUnit();
            return breakAux;
        }
    }
}