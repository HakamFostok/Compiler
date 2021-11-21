using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Compiler.Core;

public partial class Executer
{
    private BinaryFormatter formatter { get; set; }
    private TypeSymbol UL { get; set; }
    private ReturnInstruction GReturn { get; set; }
    public event EventHandler<WriteEventArgs> WriteEvent;
    public event EventHandler<WriteEventArgs> EndOfExecute;

    public Executer()
    {
        formatter = new BinaryFormatter();
    }

    public void ExecuteMainMethodFromObjectFile(string path)
    {
        using FileStream stream = new(path, FileMode.Open);
        ProcedureInstruction methods;
        try
        {
            methods = (ProcedureInstruction)formatter.Deserialize(stream);
        }
        catch (SerializationException)
        {
            GetRuntimeError(RuntimeMessagesError.ObjFile);
            return;     // this statment is unreachable. Just for the compile Error
        }

        ProcedureInstruction main = IdentifierInstruction.FindIdentifer("Main", methods);

        if (main != null)
        {
            ExecuteListOfInstructions(main.Linst);
        }
        else
        {
            GetRuntimeError(RuntimeMessagesError.NoMain);
        }

        EndOfExecute?.Invoke(this, new WriteEventArgs(true, "End of Executing... Press Enter to Exit", true));
    }

    private TypeSymbol ExecuteListOfInstructions(BaseInstruction linst)
    {
        while (linst != null)
        {
            if (linst.Ins is CallInstruction)
            {
                CallInstruction callaux = linst.Ins as CallInstruction;
                UL = ExecuteCall(callaux);

                if (UL == TypeSymbol.U_Return)
                {
                    // warning but I condiered it as error;
                    GetRuntimeError(RuntimeMessagesError.ReturnInProcedure);
                }

                if (UL == TypeSymbol.U_Halt)
                {
                    return UL;
                }
            }
            else if (linst.Ins is IfInstruction)
            {
                IfInstruction ifaux = linst.Ins as IfInstruction;
                UL = ExecuteListOfInstructions((EvaluateCondition(ifaux.Cond)) ? ifaux.Ins : ifaux.InsElse);

                if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Break or
                    TypeSymbol.U_Return or TypeSymbol.U_Exit)
                {
                    return UL;
                }
            }
            else if (linst.Ins is WhileInstruction)
            {
                WhileInstruction whileaux = linst.Ins as WhileInstruction;
                while (EvaluateCondition(whileaux.Cond))
                {
                    UL = ExecuteListOfInstructions(whileaux.Ins);

                    if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Return or
                        TypeSymbol.U_Exit)
                    {
                        return UL;
                    }

                    if (UL == TypeSymbol.U_Break)
                    {
                        break;
                    }
                }
            }
            else if (linst.Ins is ForInstruction)
            {
                ForInstruction foraux = linst.Ins as ForInstruction;
                TExpression expB = EvaluateInteger(foraux.ExpBegin);
                TExpression expE = EvaluateInteger(foraux.ExpEnd);
                TExpression step = EvaluateInteger(foraux.ExpStep);

                if (!foraux.IsDown)
                {
                    Assign(foraux.V, 0, expB);
                    while (expB.ValNB <= expE.ValNB)
                    {
                        UL = ExecuteListOfInstructions(foraux.Ins);

                        if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Return or
                            TypeSymbol.U_Exit)
                        {
                            return UL;
                        }

                        if (UL == TypeSymbol.U_Break)
                        {
                            break;
                        }

                        // expB.ValNB = expB.ValNB + 1;
                        expB.ValNB = expB.ValNB + step.ValNB;
                        Assign(foraux.V, 0, expB);
                    }
                }
                else
                {
                    Assign(foraux.V, 0, expB);
                    while (expB.ValNB >= expE.ValNB)
                    {
                        UL = ExecuteListOfInstructions(foraux.Ins);

                        if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Return or
                        TypeSymbol.U_Exit)
                        {
                            return UL;
                        }

                        if (UL == TypeSymbol.U_Break)
                        {
                            break;
                        }

                        // expB.ValNB = expB.ValNB + 1;
                        expB.ValNB = expB.ValNB - step.ValNB;
                        Assign(foraux.V, 0, expB);
                    }
                }

                Free(ref expB);
                Free(ref expE);
                Free(ref step);
            }
            else if (linst.Ins is DoWhileInstruction)
            {
#if doc
                DoWhileInstruction doaux = linst.Ins as DoWhileInstruction;
                do
                {
                    UL = ExecuteListOfInstructions(doaux.Ins);

                    if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Return or
                        TypeSymbol.U_Exit)
                    {
                        return UL;
                    }

                    if (UL == TypeSymbol.U_Break)
                    {
                        break;
                    }
                } while (EvaluateCondition(doaux.Cond));
#else
                    //ExecuteOneTimeLoopAtLeast(linst.Ins as TConditionBase, true);
#endif
            }
            else if (linst.Ins is RepeatUntilInstruction)
            {
#if doc
                RepeatUntilInstruction repeataux = linst.Ins as RepeatUntilInstruction;
                do
                {
                    UL = ExecuteListOfInstructions(repeataux.Ins);

                    if (UL is TypeSymbol.U_Halt or TypeSymbol.U_Return or
                        TypeSymbol.U_Exit)
                    {
                        return UL;
                    }

                    if (UL == TypeSymbol.U_Break)
                    {
                        break;
                    }
                } while (!EvaluateCondition(repeataux.Cond));
#else
                    ExecuteOneTimeLoopAtLeast(linst.Ins as TConditionBase ,false)
#endif
            }
            else if (linst.Ins is BreakInstruction)
            {
                return (linst.Ins as BreakInstruction).UL;
            }
            else if (linst.Ins is ReturnInstruction)
            {
                ReturnInstruction returnaux = linst.Ins as ReturnInstruction;
                GReturn = returnaux;
                return TypeSymbol.U_Return;
            }
            else if (linst.Ins is AssignInstruction)
            {
                AssignInstruction assignaux = linst.Ins as AssignInstruction;

                Func<TExpression, TExpression, TExpression> a = (expValue, op) =>
                {
                    expValue.Next = op;
                    op.Prev = expValue;

                    TExpression varExpression = new() { UL = TypeSymbol.U_Var };
                    varExpression.ValVar = assignaux.Var;
                    varExpression.Index = TExpression.CopyExpression(assignaux.index);

                    varExpression.Next = expValue;
                    expValue.Prev = varExpression;

                    return EvaluateExpression(varExpression);
                };

                TExpression exp0;
                TExpression expOperator;

                if (assignaux.Exp != null) // = , += , -= , *= , /=, ^= , %=
                {
                    exp0 = EvaluateExpression(assignaux.Exp);
                    if (assignaux.UL != TypeSymbol.U_Assignment) //  += , -= , *= , /=, ^= , %=
                    {
                        TypeSymbol ulAux;
                        if (assignaux.UL == TypeSymbol.U_PluseAssigment)
                        {
                            ulAux = TypeSymbol.U_Pluse;
                        }
                        else if (assignaux.UL == TypeSymbol.U_MinusAssigment)
                        {
                            ulAux = TypeSymbol.U_Minus;
                        }
                        else if (assignaux.UL == TypeSymbol.U_MultiplyAssigment)
                        {
                            ulAux = TypeSymbol.U_Multiply;
                        }
                        else if (assignaux.UL == TypeSymbol.U_DivisionAssigment)
                        {
                            ulAux = TypeSymbol.U_Division;
                        }
                        else if (assignaux.UL == TypeSymbol.U_PowAssigment)
                        {
                            ulAux = TypeSymbol.U_Pow;
                        }
                        else //if (assignaux.UL == TypeSymbol.U_ModAssigment)
                        {
                            ulAux = TypeSymbol.U_Mod;
                        }

                        expOperator = new TExpression { UL = ulAux };
                        exp0 = EvaluateExpression(assignaux.Exp);
                        if ((exp0.Next != null) || (exp0.Prev != null))
                        {
                            throw new ArgumentException("This is notification for me hakam");
                        }

                        expOperator = new TExpression { UL = ulAux };
                        exp0 = EvaluateExpression(assignaux.Exp);
                        if ((exp0.Next != null) || (exp0.Prev != null))
                        {
                            throw new ArgumentException("This is notification for me hakam");
                        }

                        exp0 = a(exp0, expOperator);
                    }
                }
                else // ++ , --
                {
                    exp0 = new TExpression { UL = TypeSymbol.U_Cst_Int, ValNB = 1 };
                    if (assignaux.UL == TypeSymbol.U_PlusePluse)
                    {
                        expOperator = new TExpression { UL = TypeSymbol.U_Pluse };
                    }
                    else
                    {
                        expOperator = new TExpression { UL = TypeSymbol.U_Minus };
                    }

                    exp0 = a(exp0, expOperator);
                }

                if (assignaux.index == null)
                {
                    Assign(assignaux.Var, 0, exp0);
                }
                else
                {
                    TExpression iexp = EvaluateInteger(assignaux.index);
                    Assign(assignaux.Var, GiveIntWithRuntimeError(Math.Truncate(iexp.ValNB)), exp0);
                    Free(ref iexp);
                }

                Free(ref exp0);
            }

            else if (linst.Ins is TWrite)
            {
                TWrite writeAux = linst.Ins as TWrite;
                TExpression exp = EvaluateExpression(writeAux.exp);

                string output = "";
                while (exp != null)
                {
                    if (exp.UL == TypeSymbol.U_True)
                    {
                        output += true.ToString();
                    }
                    else if (exp.UL == TypeSymbol.U_False)
                    {
                        output += false.ToString();
                    }
                    else if (exp.UL == TypeSymbol.U_Cst_Str)
                    {
                        output += exp.ValStr;
                    }
                    else if (exp.UL is TypeSymbol.U_Cst_Real or TypeSymbol.U_Cst_Int)
                    {
                        output += exp.ValNB.ToString();
                    }

                    exp = exp.Next;
                }

                WriteEvent?.Invoke(this, new WriteEventArgs(writeAux.isLn, output, false));

                Free(ref exp);
            }
            else if (linst.Ins is ReadInstruction)
            {
                ReadInstruction readAux = linst.Ins as ReadInstruction;

                InputForm inputRead = new();
                inputRead.ShowDialog();
                string result = inputRead.Input;
                TExpression expAux = new() { UL = TypeSymbol.U_Cst_Str, ValStr = result };

                try
                {
                    expAux.ValNB = Convert.ToDouble(result);

                    expAux.UL = IsIntMethod(expAux.ValNB);
                    expAux.ValStr = "";
                }
                catch (FormatException)
                {
                }

                try
                {
                    expAux.UL = (Convert.ToBoolean(result) ? TypeSymbol.U_True : TypeSymbol.U_False);
                    expAux.ValStr = "";
                }
                catch (FormatException)
                {

                }

                int index = 0;
                if (readAux.index != null)
                {
                    TExpression temp = EvaluateInteger(readAux.index);
                    index = GiveIntWithRuntimeError(Math.Truncate(temp.ValNB));
                    Free(ref temp);
                }

                Assign(readAux.V, index, expAux);
            }
            else
            {
                GetRuntimeError(RuntimeMessagesError.UnknownInstruction);
            }

            linst = linst.Next;
        }

        return TypeSymbol.U_EOF;
    }

    private TypeSymbol ExecuteCall(CallInstruction callaux)
    {
        // 1 . handle the input variables
        TExpression exp0 = EvaluateExpression(callaux.Pin);
        TVar varaux;
        if (exp0 != null)
        {
            while (exp0.Next != null)
            {
                exp0 = exp0.Next;
            }

            varaux = callaux.P.PIN;
            TExpression expaux;
            while (varaux != null && exp0 != null)
            {
                expaux = exp0.Prev;
                if (expaux != null)     // this condition I added because of expcetion.
                {
                    expaux.Next = null;
                }

                //expaux.Prev = null;
                exp0.Prev = null;

                Assign(varaux, 0, exp0);

                Free(ref exp0);
                exp0 = expaux;
                varaux = (TVar)varaux.Next;
            }

            // error the variables in method is different from variables in call.
            if (varaux != null || exp0 != null)
            {
                GetRuntimeError(RuntimeMessagesError.VarsDifferents, "inupt");
            }
        }

        // 2 . Handle the output variables.
        TListVar lv = callaux.Pout;
        varaux = callaux.P.POut;

        if (!(lv == null && varaux == null))
        {
            if (varaux == null)
            {
                GetRuntimeError(RuntimeMessagesError.VarsDifferents, "output");
            }
            while (lv != null && varaux != callaux.P.PIN)
            {
                AssignVar(varaux, lv.V);
                varaux = (TVar)varaux.Next;
                lv = lv.Next;
            }

            // error the output variables in method is different from output variables in call
            if (lv != null || varaux != callaux.P.PIN)
            {
                GetRuntimeError(RuntimeMessagesError.VarsDifferents, "output");
            }
        }

        // 3 . Execute the Instructions of method.
        UL = ExecuteListOfInstructions(callaux.P.Linst);

        // 4 . return the output vaiables from the call to the orgianl variables in the methods.
        lv = callaux.Pout;
        varaux = callaux.P.POut;

        if (varaux == null)
        {
            varaux = callaux.P.PIN;
        }

        // 5 . return the input variables from the call to the orginal vaiables in the methods.
        while (lv != null) // here there is another condition
        {
            AssignVar(lv.V, varaux);
            varaux = (TVar)varaux.Next;
            lv = lv.Next;
        }

        return UL;
    }

    private static void AssignVar(TVar left, TVar right)
    {
        FreeItems(left);
        TItem last = null;
        TItem itemaux = right.items;
        while (itemaux != null)
        {
            TItem newItem = new()
            {
                UL = itemaux.UL,
                ValNB = itemaux.ValNB,
                ValStr = itemaux.ValStr
            };
            if (left.items == null)
            {
                left.items = newItem;
            }
            else
            {
                last.Next = newItem;
            }

            last = newItem;

            itemaux = itemaux.Next;
        }
    }

    private TExpression EvaluateExpression(TExpression exp)
    {
        TExpression exp0 = TExpression.CopyExpression(exp);
        TExpression currexp = exp0;

        while (currexp != null)
        {
            TypeSymbol currExpUL = currexp.UL;

            if (currExpUL is TypeSymbol.U_Cst_Str or TypeSymbol.U_Cst_Int or TypeSymbol.U_Cst_Real
                or TypeSymbol.U_True or TypeSymbol.U_False)
            {
                currexp = currexp.Next;
            }
            else if (currexp.UL == TypeSymbol.U_VarProcedure)
            {
                CallInstruction callaux = currexp.ValCall;
                UL = ExecuteCall(callaux);
                if (UL != TypeSymbol.U_Return)
                {
                    GetRuntimeError(RuntimeMessagesError.NoReturnInFunction);
                }

                TExpression expAux = EvaluateExpression(GReturn.Exp);
                currexp.UL = expAux.UL;
                currexp.ValNB = expAux.ValNB;
                currexp.ValStr = expAux.ValStr;
                Free(ref expAux);
            }
            else if (currExpUL == TypeSymbol.U_Var)
            {
                // if the variables has index.
                int i = 1;
                if (currexp.Index != null)
                {
                    TExpression expAux = EvaluateInteger(currexp.Index);
                    i = GiveIntWithRuntimeError(Math.Truncate(expAux.ValNB));
                    Free(ref expAux);
                }

                TItem itemAux = currexp.ValVar.items;
                while ((itemAux != null) && (i > 1))
                {
                    i--;
                    itemAux = itemAux.Next;
                }

                if (itemAux == null)
                {
                    currexp.UL = TypeSymbol.U_UnKown;
                }
                else
                {
                    currexp.UL = itemAux.UL;
                    currexp.ValNB = itemAux.ValNB;
                    currexp.ValStr = itemAux.ValStr;
                }
            }

            // the functions that take one parameter.
            else if (currExpUL is TypeSymbol.U_Not or TypeSymbol.U_UnaryPluse or TypeSymbol.U_UnaryMinuse
                or TypeSymbol.U_Sin or TypeSymbol.U_Cos or TypeSymbol.U_Tg or TypeSymbol.U_Ln
                or TypeSymbol.U_Log or TypeSymbol.U_Exp or TypeSymbol.U_Sqr or TypeSymbol.U_Sqrt
                or TypeSymbol.U_Length or TypeSymbol.U_IntToStr or TypeSymbol.U_StrToInt
                or TypeSymbol.U_IntToHex)
            {
                TypeSymbol ulAux = currexp.Prev.UL;
                TExpression prevExp = currexp.Prev;

                Action IsInt = () => prevExp.UL = IsIntMethod(prevExp.ValNB);

                // validation
                if ((currExpUL == TypeSymbol.U_UnaryMinuse || currExpUL == TypeSymbol.U_UnaryPluse
                  || currExpUL == TypeSymbol.U_Sin || currExpUL == TypeSymbol.U_Cos || currExpUL == TypeSymbol.U_Tg
                  || currExpUL == TypeSymbol.U_Ln || currExpUL == TypeSymbol.U_Log || currExpUL == TypeSymbol.U_Exp
                  || currExpUL == TypeSymbol.U_Sqr || currExpUL == TypeSymbol.U_Sqrt || currExpUL == TypeSymbol.U_IntToStr)
                    && (ulAux != TypeSymbol.U_Cst_Int && ulAux != TypeSymbol.U_Cst_Real))
                {
                    GetRuntimeError(RuntimeMessagesError.InvalideType, ulAux.ToString(), currExpUL.ToString());
                }
                if ((currExpUL == TypeSymbol.U_Not) && (ulAux != TypeSymbol.U_True && ulAux != TypeSymbol.U_False))
                {
                    GetRuntimeError(RuntimeMessagesError.InvalideType, ulAux.ToString(), currExpUL.ToString());
                }
                if ((currExpUL == TypeSymbol.U_StrToInt || currExpUL == TypeSymbol.U_Length) && ulAux != TypeSymbol.U_Cst_Str)
                {
                    GetRuntimeError(RuntimeMessagesError.InvalideType, ulAux.ToString(), currExpUL.ToString());
                }
                if (currExpUL == TypeSymbol.U_IntToHex && (ulAux != TypeSymbol.U_Cst_Int))
                {
                    GetRuntimeError(RuntimeMessagesError.InvalideType, ulAux.ToString(), currExpUL.ToString());
                }

                if (currExpUL == TypeSymbol.U_UnaryPluse)
                {
                    // nothing will change
                }
                else if (currExpUL == TypeSymbol.U_UnaryMinuse)
                {
                    prevExp.ValNB = -prevExp.ValNB;
                }
                else if (currExpUL == TypeSymbol.U_IntToHex)
                {
                    string result = "";
                    int number = Convert.ToInt32(prevExp.ValNB);
                    if (number == 0)
                    {
                        result = "0x0";
                    }
                    else
                    {
                        string mod = "";
                        while (number != 0)
                        {
                            int modInt = (number % 16);

                            if (modInt <= 9)
                            {
                                mod = modInt.ToString();
                            }
                            else
                            {
                                mod = LexicalAnalyst.invValues[modInt].ToString();
                            }

                            result = mod + result;
                            number = number / 16;
                        }

                        result = "0x" + result;
                    }

                    prevExp.ValStr = result;
                    prevExp.ValNB = 0;
                    prevExp.UL = TypeSymbol.U_Cst_Str;
                }
                else if (currExpUL == TypeSymbol.U_Sin)
                {
                    prevExp.ValNB = Math.Sin(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Cos)
                {
                    prevExp.ValNB = Math.Cos(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Tg)
                {
                    prevExp.ValNB = Math.Tan(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Ln)
                {
                    prevExp.ValNB = Math.Log(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Log)
                {
                    prevExp.ValNB = Math.Log10(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Exp)
                {
                    prevExp.ValNB = Math.Exp(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Sqr)
                {
                    prevExp.ValNB = Math.Pow(prevExp.ValNB, 2);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Sqrt)
                {
                    prevExp.ValNB = Math.Sqrt(prevExp.ValNB);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Not)
                {
                    prevExp.UL = (prevExp.UL == TypeSymbol.U_True) ? TypeSymbol.U_False : TypeSymbol.U_True;
                }
                else if (currExpUL == TypeSymbol.U_IntToStr)
                {
                    prevExp.ValStr = prevExp.ValNB.ToString();
                    prevExp.UL = TypeSymbol.U_Cst_Str;
                }
                else if (currExpUL == TypeSymbol.U_StrToInt)
                {
                    prevExp.ValNB = Convert.ToDouble(prevExp.ValStr);
                    IsInt();
                }
                else if (currExpUL == TypeSymbol.U_Length)
                {
                    prevExp.ValNB = prevExp.ValStr.Length;
                    prevExp.UL = TypeSymbol.U_Cst_Int;
                }

                // Free the expressions that used
                if (currexp.Next != null)
                {
                    currexp.Next.Prev = prevExp;
                }
                prevExp.Next = currexp.Next;

                TExpression expAux = currexp.Next;
                Free(ref currexp);
                currexp = expAux;

                // the following statment is instead of 
                //currexp = currexp.Next; 
            }
            else if (currExpUL is TypeSymbol.U_And or TypeSymbol.U_Or or TypeSymbol.U_Pluse
                or TypeSymbol.U_Minus or TypeSymbol.U_Multiply or TypeSymbol.U_IntDiv
                or TypeSymbol.U_Division or TypeSymbol.U_Pow or TypeSymbol.U_Mod
                or TypeSymbol.U_Equal or TypeSymbol.U_NotEqual or TypeSymbol.U_LessEqual
                or TypeSymbol.U_LessThan or TypeSymbol.U_GreaterEqual or TypeSymbol.U_GreaterThan)
            {
                TExpression prevExp = currexp.Prev;
                TExpression prevPrevExp = prevExp.Prev;

                Action setReal = () =>
                {
                    if (prevPrevExp.UL != prevExp.UL)
                    {
                        prevPrevExp.UL = TypeSymbol.U_Cst_Real;
                    }
                };

                // validation 1
                if ((currExpUL == TypeSymbol.U_And || currExpUL == TypeSymbol.U_Or) &&
                    ((prevExp.UL != TypeSymbol.U_True && prevExp.UL != TypeSymbol.U_False) ||
                    (prevPrevExp.UL != TypeSymbol.U_True && prevPrevExp.UL != TypeSymbol.U_False)))
                {
                    GetRuntimeError(RuntimeMessagesError.ValidType, "boolean", "And, Or");
                }

                if ((currExpUL == TypeSymbol.U_LessEqual || currExpUL == TypeSymbol.U_LessThan
                    || currExpUL == TypeSymbol.U_GreaterEqual || currExpUL == TypeSymbol.U_GreaterThan
                    || currExpUL == TypeSymbol.U_Multiply || currExpUL == TypeSymbol.U_Division
                    || currExpUL == TypeSymbol.U_Mod || currExpUL == TypeSymbol.U_Pow
                    || currExpUL == TypeSymbol.U_Minus) &&
                    ((prevExp.UL != TypeSymbol.U_Cst_Int && prevExp.UL != TypeSymbol.U_Cst_Real) ||
                    (prevPrevExp.UL != TypeSymbol.U_Cst_Int && prevPrevExp.UL != TypeSymbol.U_Cst_Real)))
                {
                    GetRuntimeError(RuntimeMessagesError.ValidType, "int , double", "<, <=, >=, >, *, /, -, %, ^");
                }

                if (currExpUL == TypeSymbol.U_IntDiv && ((prevExp.UL != TypeSymbol.U_Cst_Int) || (prevPrevExp.UL != TypeSymbol.U_Cst_Int)))
                {
                    GetRuntimeError(RuntimeMessagesError.ValidType, "int", "div");
                }

                if ((currExpUL == TypeSymbol.U_Pluse) &&
                    ((prevExp.UL != TypeSymbol.U_Cst_Int && prevExp.UL != TypeSymbol.U_Cst_Real && prevExp.UL != TypeSymbol.U_Cst_Str) ||
                    (prevPrevExp.UL != TypeSymbol.U_Cst_Int && prevPrevExp.UL != TypeSymbol.U_Cst_Real && prevPrevExp.UL != TypeSymbol.U_Cst_Str)))
                {
                    GetRuntimeError(RuntimeMessagesError.ValidType, "int, double ,string", "+");
                }

                if (currExpUL == TypeSymbol.U_And)
                {
                    prevPrevExp.UL = (prevPrevExp.UL == TypeSymbol.U_True && prevExp.UL == TypeSymbol.U_True) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_Or)
                {
                    prevPrevExp.UL = (prevPrevExp.UL == TypeSymbol.U_True || prevExp.UL == TypeSymbol.U_True) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_Equal)
                {
                    if (IsBoolean(prevExp.UL) && IsBoolean(prevPrevExp.UL))
                    {
                        prevPrevExp.UL = (prevPrevExp.UL == prevExp.UL) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else if (IsNumber(prevExp.UL) && IsNumber(prevPrevExp.UL))
                    {
                        prevPrevExp.UL = (prevPrevExp.ValNB == prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else if (prevExp.UL == TypeSymbol.U_Cst_Str && prevPrevExp.UL == TypeSymbol.U_Cst_Str)
                    {
                        prevPrevExp.UL = (prevPrevExp.ValStr == prevExp.ValStr) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else
                    {
                        GetRuntimeError(RuntimeMessagesError.Uncompiable, "=");
                    }
                }
                else if (currExpUL == TypeSymbol.U_NotEqual)
                {
                    if (IsBoolean(prevExp.UL) && IsBoolean(prevPrevExp.UL))
                    {
                        prevPrevExp.UL = (prevPrevExp.UL != prevExp.UL) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else if (IsNumber(prevExp.UL) && IsNumber(prevPrevExp.UL))
                    {
                        prevPrevExp.UL = (prevPrevExp.ValNB != prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else if (prevExp.UL == TypeSymbol.U_Cst_Str && prevPrevExp.UL == TypeSymbol.U_Cst_Str)
                    {
                        prevPrevExp.UL = (prevPrevExp.ValStr != prevExp.ValStr) ? TypeSymbol.U_True : TypeSymbol.U_False;
                    }
                    else
                    {
                        GetRuntimeError(RuntimeMessagesError.Uncompiable, "=");
                    }
                }
                else if (currExpUL == TypeSymbol.U_LessEqual)
                {
                    prevPrevExp.UL = (prevPrevExp.ValNB <= prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_LessThan)
                {
                    prevPrevExp.UL = (prevPrevExp.ValNB < prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_GreaterEqual)
                {
                    prevPrevExp.UL = (prevPrevExp.ValNB >= prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_GreaterThan)
                {
                    prevPrevExp.UL = (prevPrevExp.ValNB > prevExp.ValNB) ? TypeSymbol.U_True : TypeSymbol.U_False;
                }
                else if (currExpUL == TypeSymbol.U_Pluse)
                {
                    if (prevExp.UL == TypeSymbol.U_Cst_Str && prevPrevExp.UL == TypeSymbol.U_Cst_Str)
                    {
                        prevPrevExp.ValStr += prevExp.ValStr;
                    }
                    else if (IsNumber(prevExp.UL) && IsNumber(prevPrevExp.UL))
                    {
                        prevPrevExp.ValNB += prevExp.ValNB;
                        setReal();
                    }
                    else
                    {
                        GetRuntimeError(RuntimeMessagesError.intStringPluse);
                    }
                }
                else if (currExpUL == TypeSymbol.U_Minus)
                {
                    prevPrevExp.ValNB = prevPrevExp.ValNB - prevExp.ValNB;
                    setReal();
                }
                else if (currExpUL == TypeSymbol.U_Multiply)
                {
                    prevPrevExp.ValNB = prevPrevExp.ValNB * prevExp.ValNB;
                    setReal();
                }
                else if (currExpUL == TypeSymbol.U_Division)
                {
                    try
                    {
                        prevPrevExp.ValNB = (double)prevPrevExp.ValNB / prevExp.ValNB;
                        setReal();
                    }
                    catch (DivideByZeroException)
                    {
                        GetRuntimeError(RuntimeMessagesError.DivByZero);
                    }
                }
                else if (currExpUL == TypeSymbol.U_Mod)
                {
                    try
                    {
                        prevPrevExp.ValNB %= prevExp.ValNB;
                        setReal();
                    }
                    catch (DivideByZeroException)
                    {
                        GetRuntimeError(RuntimeMessagesError.DivByZero);
                    }
                }
                else if (currExpUL == TypeSymbol.U_Pow)
                {
                    prevPrevExp.ValNB = Math.Pow(prevPrevExp.ValNB, prevExp.ValNB);
                    setReal();
                }
                else if (currExpUL == TypeSymbol.U_IntDiv)
                {
                    try
                    {
                        prevPrevExp.ValNB = GiveIntWithRuntimeError(prevPrevExp.ValNB) / GiveIntWithRuntimeError(prevExp.ValNB);
                        prevPrevExp.UL = TypeSymbol.U_Cst_Int;

                    }
                    catch (DivideByZeroException)
                    {
                        GetRuntimeError(RuntimeMessagesError.DivByZero);
                    }
                }

                // free the expression that used.
                TExpression expAux = prevPrevExp;
                if (currexp.Next != null)
                {
                    currexp.Next.Prev = expAux;
                }
                expAux.Next = currexp.Next;
                currexp.Prev = null;    // free the currexp.Prev
                Free(ref currexp);
                currexp = expAux.Next;
            }
            else if (currExpUL == TypeSymbol.U_UnKown)
            {
                GetRuntimeError(RuntimeMessagesError.UnassigedVariable);
            }
        }   // end while

        return exp0;
    }

    private bool EvaluateCondition(TExpression exp)
    {
        TExpression exp0 = EvaluateExpression(exp);
        if (exp0.UL is not TypeSymbol.U_True and not TypeSymbol.U_False)
        {
            GetRuntimeError(RuntimeMessagesError.NotValid, "Condition");
        }

        bool result = (exp0.UL == TypeSymbol.U_True);
        Free(ref exp0);
        return result;
    }

    private TExpression EvaluateInteger(TExpression exp)
    {
        TExpression exp0 = EvaluateExpression(exp);
        if (exp0.UL != TypeSymbol.U_Cst_Int)
        {
            GetRuntimeError(RuntimeMessagesError.NotValid, "Integer");
        }

        return exp0;
    }

    private static void Assign(TVar v, int index, TExpression exp)
    {
        if (index != 0)
        {
            int i = 0;
            TItem itemAux = v.items;
            TItem lastItem = null;
            while (i < index)
            {
                if (itemAux == null)
                {
                    TItem newItem = new() { UL = TypeSymbol.U_UnKown };

                    if (lastItem == null)
                    {
                        v.items = newItem;
                    }
                    else
                    {
                        lastItem.Next = newItem;
                    }

                    lastItem = newItem;
                }
                else
                {
                    lastItem = itemAux;
                    itemAux = itemAux.Next;
                }

                i++;
            }

            lastItem.UL = exp.UL;
            lastItem.ValNB = exp.ValNB;
            lastItem.ValStr = exp.ValStr;

            // check if we free it outside the method
            Free(ref exp);
        }
        else
        {
            FreeItems(v);
            v.items = new TItem { UL = exp.UL, ValNB = exp.ValNB, ValStr = exp.ValStr };
            //v.items.UL = exp.UL;
            //v.items.ValNB = exp.ValNB;
            //v.items.ValStr = exp.ValStr;
            Free(ref exp);
        }
    }

    private static void FreeItems(TVar v)
    {
        if (v.items != null)
        {
            TItem itemAux = v.items;
            TItem itemNext = null;

            while (itemAux != null)
            {
                itemNext = itemAux.Next;
                itemAux = null;      // itemAux.Free();
                itemAux = itemNext;
            }

            v.items = null;
        }
    }

    private static void Free(ref TExpression exp0) => exp0 = null;

    private static bool IsBoolean(TypeSymbol ul) => (ul is TypeSymbol.U_True or TypeSymbol.U_False);

    private static bool IsNumber(TypeSymbol ul) => (ul is TypeSymbol.U_Cst_Int or TypeSymbol.U_Cst_Real);

    private static TypeSymbol IsIntMethod(double number)
    {
        try
        {
            return (Convert.ToInt32(number) == number) ? TypeSymbol.U_Cst_Int : TypeSymbol.U_Cst_Real;
        }
        catch (OverflowException)
        {
            GetRuntimeError(RuntimeMessagesError.ToBigOnInt);
        }
        return TypeSymbol.U_Cst_Real;
    }

    private static int GiveIntWithRuntimeError(double number)
    {
        try
        {
            int i = Convert.ToInt32(number);
            return i;
        }
        catch (OverflowException)
        {
            GetRuntimeError(RuntimeMessagesError.ToBigOnInt);
        }

        // this statment will not ever execute.
        return 0;
    }

    private static void GetRuntimeError(string fullMessage, params string[] parameters) => MakeRuntimeError(string.Format(fullMessage, parameters));

    private static void MakeRuntimeError(string fullMessage) => throw new RuntimeErrorException(fullMessage);
}
