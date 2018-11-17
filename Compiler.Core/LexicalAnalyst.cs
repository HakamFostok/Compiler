using System;
using System.Collections.Generic;

namespace Compiler.Core
{
    partial class LexicalAnalyst
    {
        private string id { get; set; }
        private Variables Locals { get; set; }
        private TypeSymbol UL { get; set; }

        private string[] CF
        {
            get
            {
                return Locals.CF;
            }
        }

        private int CI
        {
            get
            {
                return Locals.CI;
            }
            set
            {
                Locals.CI = value;
            }
        }

        private int lineNumber
        {
            get
            {
                return Locals.lineNumber;
            }
            set
            {
                Locals.lineNumber = value;
            }
        }

        private TFile gFile
        {
            get
            {
                return Locals.Gfile;
            }
        }

        private TVar gVar
        {
            get
            {
                return Locals.Gvar;
            }
        }

        private TDefine gDefine
        {
            get
            {
                return Locals.Gdefine;
            }
        }

        private char CC
        {
            get
            {
                return CL[CI];
            }
        }

        private char NextCC
        {
            get
            {
                return CL[CI + 1];
            }
        }

        private bool CCInLine
        {
            get
            {
                return CI < CL.Length;
            }
        }

        private bool NextCCInLine
        {
            get
            {
                return CI + 1 < CL.Length;
            }
        }

        private TProcedure gProc
        {
            get
            {
                return Locals.GPorc;
            }
        }

        private TFile currFile
        {
            get
            {
                return Locals.CurrFile;
            }
        }

        private double G_curr_Num
        {
            get
            {
                return Locals.G_curr_NB;
            }
            set
            {
                Locals.G_curr_NB = value;
            }
        }

        private string G_curr_Str
        {
            get
            {
                return Locals.G_curr_str;
            }
            set
            {
                Locals.G_curr_str = value;
            }
        }

        private TIdentifier G_curr_ID
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

#if doc
        private string CL
        {
            get
            {
                return Locals.CL;
            }
            set
            {
                Locals.CL = value;
            }
        }

#else
      
        private string CL
        {
            get
            {
                return CF[lineNumber];
            }
        }

        internal List<TypeSymbol> token { get; set; }
        private static Dictionary<char, TypeSymbol> valuesOfChars { get; set; }

        static LexicalAnalyst()
        {
            valuesOfChars = new Dictionary<char, TypeSymbol> 
            {
                { '(' , TypeSymbol.U_OpenParanthese },
                { ')' , TypeSymbol.U_ClosedParanthese },
                { '[' , TypeSymbol.U_OpenBracket },
                { ']' , TypeSymbol.U_ClosedBracket },
                { '=' , TypeSymbol.U_Equal },
                { '+' , TypeSymbol.U_Pluse },
                { '-' , TypeSymbol.U_Minus },
                { '*' , TypeSymbol.U_Multiply },
                { '/' , TypeSymbol.U_Division },
                { '!' , TypeSymbol.U_Not },
                { '^' , TypeSymbol.U_Pow },
                { ';' , TypeSymbol.U_SemiColon },
                { ',' , TypeSymbol.U_Comma },
                { '%' , TypeSymbol.U_Mod },
                { '{' , TypeSymbol.U_OpenBraces },
                { '}' , TypeSymbol.U_ClosedBraces }
            };
        }
        
        internal void GetTokensOfFile(TFile file)
        {
            InitializeLexerForNewFile();
            PreProcess();
            TypeSymbol temp = LexicalUnit();
            while (LexicalUnit() != TypeSymbol.U_EOF)
            {
                token.Add(temp);
                temp = LexicalUnit();
            }
        }

        private string[] GetWordsFromFileWithOutComment()
        {
            char[] splitChars = new char[] { ' ' };
            bool inComment = false;

            string[] temps = CF.Select(line => new string(line.TakeWhile((c, index) =>
                 !(c == '/' && index + 1 < line.Length && line[index + 1] == '/')).ToArray())).
                 SelectMany(line => line.Split(splitChars)).Where(str => str.Length > 0).ToArray();
            
            return temps.Where(str =>
                  {
                      bool startComment = (str.StartsWith("/*"));
                      bool endComment = (str.EndsWith("*/"));
                      bool result = !inComment && !startComment;
                      inComment = (!endComment) && ((!inComment && startComment) || inComment);
                      return result;
                  }).ToArray();

            // for more information about the logical formual of the boolean variable 'inComment' and 'result'
            // see the excel file assocaited with the project 
            // this file demostrate how did we get this functions.
        }

#endif
        public LexicalAnalyst(Variables loc)
        {
            Locals = loc;
        }

        private void GetMacroError(string message, params string[] parameters)
        {
            MakeMacroError(string.Format(message, parameters));
        }

        private void GetMacroError(string message)
        {
            MakeMacroError(message);
        }

        private void MakeMacroError(string fullMessage)
        {
            throw new MacroErrorException(fullMessage, CI, lineNumber, currFile.Name);
        }

        private void GetWordError(string message, params string[] parameters)
        {
            MakeWordError(string.Format(message, parameters));
        }

        private void GetWordError(string message)
        {
            MakeWordError(message);
        }

        private TypeSymbol GetWordErrorWihtReturn(string message, params string[] parameters)
        {
            GetWordError(message, parameters);
            return TypeSymbol.U_Error;
        }

        private void MakeWordError(string fullMessage)
        {
            throw new LexicalErrorException(fullMessage, CI, lineNumber, currFile.Name);
        }

        internal void PreProcess()
        {
            if (!ReadNewLine())
            {
                GetWordError(WordMessagesError.FileEmpty, currFile.Name);
            }

            if (!SkipSpacesAndComment())
            {
                GetWordError(WordMessagesError.LackFileCode, currFile.Name);
            }

            while (CC == '#')
            {
                if ((NextCCInLine) && (char.ToLower(NextCC) == 'i' || char.ToLower(NextCC) == 'd'))
                {
                    CI++;
                    UL = LexicalUnit();
                    if (UL == TypeSymbol.U_Include)
                    {
                        SkipSpaces();
                        if (!(CCInLine && CC == '\''))
                        {
                            GetMacroError(WordMessagesError.NotFound, "string");
                        }
                        UL = LexicalUnit();
                        if (UL != TypeSymbol.U_Cst_Str)
                        {
                            GetMacroError(WordMessagesError.NotFound, "string");
                        }

                        TFile.AddFile(G_curr_Str, gFile);
                    }
                    else if (UL == TypeSymbol.U_Define)
                    {
                        SkipSpaces();

                        if (!((CCInLine) && (char.IsLetter(CC) || CC == '_')))
                        {
                            GetMacroError(WordMessagesError.NotFound, "Identifier");
                        }

                        UL = LexicalUnit();

                        if (UL != TypeSymbol.U_UnKown && UL != TypeSymbol.U_VarProcedure)
                        {
                            GetMacroError(WordMessagesError.IdKnown);
                        }

                        if (UL == TypeSymbol.U_UnKown)
                        {
                            TIdentifier.AddIdentifier(G_curr_Str, ref Locals.gdefine);
                        }
                        if (UL == TypeSymbol.U_VarProcedure)
                        {
                            TIdentifier.AddIdentifier(id.ToUpper(), ref Locals.gdefine);
                        }

                        TDefine defAux = gDefine;
                        SkipSpaces();

                        if (!(CCInLine && (CC == '.' || char.IsNumber(CC) || CC == '\'')))
                        {
                            GetMacroError(WordMessagesError.NotFound, "number or string");
                        }

                        UL = LexicalUnit();
                        if (UL == TypeSymbol.U_Cst_Int || UL == TypeSymbol.U_Cst_Real)
                        {
                            defAux.ValNB = G_curr_Num;
                        }
                        else if (UL == TypeSymbol.U_Cst_Str)
                        {
                            defAux.ValStr = G_curr_Str;
                        }
                        else
                        {
                            GetMacroError(WordMessagesError.NotValidChar, "number or string");
                        }

                        defAux.UL = UL;
                    }
                    else
                    {
                        GetMacroError(WordMessagesError.NotFound, "Include or Define");
                    }
                }
                else
                {
                    GetMacroError(WordMessagesError.NoSpace);
                }
            }

            //return UL;
        }

        private bool ReadNewLine()
        {
            while (lineNumber < CF.Length)
            {
                CL = CF[lineNumber];
                // weed must not invoke the trim method if we want to get the right column number of the file. 
                lineNumber++;
                if (CL.Length > 0)
                {
                    CI = 0;
                    return true;
                }
            }

            return false;
        }

        private void SkipSpaces()
        {
            // skip the spaces
            while (CCInLine && /* char.IsWhiteSpace(CC) */ CC == ' ')
            {
                CI++;
            }
        }

        private bool SkipSpacesAndComment()
        {
            do
            {
                while (CCInLine && /* char.IsWhiteSpace(CC) */  CC == ' ')
                {
                    CI++;
                }
                if (CI == CL.Length)
                {
                    if (!ReadNewLine())
                    {
                        return false;
                    }
                }
                // here we should make else srounded the reminder of the code;
                else
                {
                    if (CI + 1 == CL.Length)
                    {
                        return true;
                    }

                    if (CC == '/' && NextCC == '/')
                    {
                        if (!ReadNewLine())
                        {
                            return false;
                        }
                    }
                    else if (CC == '/' && NextCC == '*')
                    {
                        CI += 2;
                        while (true)
                        {
                            while (NextCCInLine && !(CC == '*' && NextCC == '/'))
                            {
                                CI++;
                            }

                            if (CI + 1 >= CL.Length) // */ not found
                            {
                                if (!ReadNewLine())
                                {
                                    return false;
                                }
                            }
                            else // */ found
                            {
                                CI += 2;
                                break;
                            }
                        }   // end while
                    }   // end if
                    else
                    {
                        return true;
                    }
                }
            } while (true);
        }

        internal TypeSymbol LexicalUnit()
        {
            Func<TypeSymbol, TypeSymbol, TypeSymbol> compoundAssigment = (without, with) =>
            {
                CI++;
                if (CCInLine && CC == ':')
                {
                    CI++;
                    if (CCInLine && CC == '=')
                    {
                        CI++;
                        return with;
                    }
                    else
                    {
                        GetWordError(WordMessagesError.NotFound, "=");
                    }

                }
                return without;
            };

            Func<TypeSymbol, TypeSymbol, TypeSymbol, TypeSymbol> incrementDecrement = (without, with, incDec) =>
                {
                    CI++;
                    if (CCInLine && CC == ':')
                    {
                        CI++;
                        if (CCInLine && CC == '=')
                        {
                            return with;
                        }
                        else
                        {
                            GetWordError(WordMessagesError.NotFound, "=");
                        }
                    }
                    if (CCInLine && CC == '+')
                    {
                        return TypeSymbol.U_PlusePluse;
                    }

                    return without;
                };

            if (!SkipSpacesAndComment())
            {
                return TypeSymbol.U_EOF;
            }

            if (char.IsNumber(CC) || CC == '.')
            {
                return HandleNumber();
            }
            else if (CC == '\'')
            {
                return HandleString();
            }
            else if (char.IsLetter(CC) || CC == '_')
            {
                return HandleIdentifier();
            }

            else if (CC == '<')
            {
                CI++;
                if (CCInLine && CC == '=')
                {
                    CI++;
                    return TypeSymbol.U_LessEqual;
                }
                if (CCInLine && CC == '>')
                {
                    CI++;
                    return TypeSymbol.U_NotEqual;
                }

                if (CCInLine)
                {
                    if (CC == '=')
                    {
                        CI++;
                        return TypeSymbol.U_LessEqual;
                    }
                    if (CC == '>')
                    {
                        CI++;
                        return TypeSymbol.U_NotEqual;
                    }
                }

                return TypeSymbol.U_LessThan;
            }
            else if (CC == '>') // not worted by doctor
            {
                CI++;
                if (CCInLine && CC == '=')
                {
                    CI++;
                    return TypeSymbol.U_GreaterEqual;
                }

                return TypeSymbol.U_GreaterThan;
            }
            else if (CC == ':') // not worted by doctor
            {
                CI++;
                if (CCInLine && CC == '=')
                {
                    CI++;
                    return TypeSymbol.U_Assignment;
                }

                return GetWordErrorWihtReturn(WordMessagesError.NotFound, "=");
            }
            else if (CC == '&')
            {
                CI++;
                if (CCInLine && CC == '&')
                {
                    CI++;
                    return TypeSymbol.U_And;
                }

                return GetWordErrorWihtReturn(WordMessagesError.NotFound, "&");
            }
            else if (CC == '|')
            {
                CI++;
                if (CCInLine && CC == '|')
                {
                    CI++;
                    return TypeSymbol.U_Or;
                }

                return GetWordErrorWihtReturn(WordMessagesError.NotFound, "|");
            }
            else if (CC == '(') // not wroted by doctor
            {
                CI++;
                return TypeSymbol.U_OpenParanthese;
            }
            else if (CC == ')')
            {
                CI++;
                return TypeSymbol.U_ClosedParanthese;
            }
            else if (CC == '=')
            {
                CI++;
                return TypeSymbol.U_Equal;
            }
            else if (CC == ';') // not wroted by doctor
            {
                CI++;
                return TypeSymbol.U_SemiColon;
            }
            else if (CC == ',') // not wroted by doctor
            {
                CI++;
                return TypeSymbol.U_Comma;
            }
            else if (CC == '!') // not wroted by doctor
            {
                CI++;
                return TypeSymbol.U_Not;
            }
            else if (CC == '+')
            {
                CI++;
                if (CCInLine && CC == ':')
                {
                    CI++;
                    if (CCInLine && CC == '=')
                    {
                        CI++;
                        return TypeSymbol.U_PluseAssigment;
                    }
                    else
                    {
                        GetWordError(WordMessagesError.NotFound, "=");
                    }
                }
                if (CCInLine && CC == '+')
                {
                    CI++;
                    return TypeSymbol.U_PlusePluse;
                }

                return TypeSymbol.U_Pluse;
            }
            else if (CC == '-')
            {
                CI++;
                if (CCInLine && CC == ':')
                {
                    CI++;
                    if (CCInLine && CC == '=')
                    {
                        CI++;
                        return TypeSymbol.U_MinusAssigment;
                    }
                    else
                    {
                        GetWordError(WordMessagesError.NotFound, "=");
                    }
                }
                if (CCInLine && CC == '-')
                {
                    CI++;
                    return TypeSymbol.U_MinusMinus;
                }

                return TypeSymbol.U_Minus;
            }
            else if (CC == '*')
            {
                return compoundAssigment(TypeSymbol.U_Multiply, TypeSymbol.U_MultiplyAssigment);
            }
            else if (CC == '/')
            {
                return compoundAssigment(TypeSymbol.U_Division, TypeSymbol.U_DivisionAssigment);
            }
            else if (CC == '%')
            {
                return compoundAssigment(TypeSymbol.U_Mod, TypeSymbol.U_ModAssigment);
            }
            else if (CC == '^')
            {
                CI++;
                if (CCInLine && CC == '^')
                {
                    CI++;
                    return TypeSymbol.U_XorBitWise;
                }
                if (CCInLine && CC == ':')
                {

                    CI++;
                    if (CCInLine && CC == '=')
                    {
                        CI++;
                        return TypeSymbol.U_PowAssigment;
                    }
                    else
                    {
                        GetWordError(WordMessagesError.NotFound, "=");
                    }

                }
                return TypeSymbol.U_Pow; ;
            }
            else if (CC == '}')
            {
                CI++;
                return TypeSymbol.U_ClosedBraces;
            }
            else if (CC == '{')
            {
                CI++;
                return TypeSymbol.U_OpenBraces;
            }
            else if (CC == ']')
            {
                CI++;
                return TypeSymbol.U_ClosedBracket;
            }
            else if (CC == '[')
            {
                CI++;
                return TypeSymbol.U_OpenBracket;
            }
            else if (CC == '~')
            {
                CI++;
                return TypeSymbol.U_Complement;
            }
            else
            {
                return GetWordErrorWihtReturn(WordMessagesError.NotValidChar, CC.ToString());
            }
        }

        private TypeSymbol HandleIdentifier()
        {
            id = CC.ToString();
            CI++;

            while (CCInLine && (char.IsNumber(CC) || char.IsLetter(CC) || CC == '_'))
            {
                id += CC;
                CI++;
            }

            TSymbol symAux = TSymbol.FindSymbol(id, AubCompiler.Gsymbol);
            if (symAux != null)
            {
                return symAux.UL;
            }

            string idUpperCase = id.ToUpper();
            G_curr_ID = TIdentifier.FindIdentifer(idUpperCase, gVar);
            if (G_curr_ID != null)
            {
                return TypeSymbol.U_Var;
            }

            G_curr_ID = TIdentifier.FindIdentifer(idUpperCase, gDefine);
            if (G_curr_ID != null)
            {
                return TypeSymbol.U_VarDefine;
            }

            G_curr_ID = TIdentifier.FindIdentifer(idUpperCase, gProc);
            if (G_curr_ID != null)
            {
                return TypeSymbol.U_VarProcedure;
            }

            G_curr_Str = id.ToUpper();
            return TypeSymbol.U_UnKown;
        }

        private TypeSymbol HandleString()
        {
            CI++;
            G_curr_Str = "";

            while (CCInLine && CC != '\'')
            {
                if (CC == '\\')
                {
                    if (NextCCInLine && (NextCC == '\\' || NextCC == '\''))
                    {
                        G_curr_Str += NextCC;
                        CI += 2;
                    }
                    else
                    {
                        GetWordError(WordMessagesError.EscapeSequence);
                    }
                }
                else
                {
                    G_curr_Str += CC;
                    CI++;
                }
            }

            if (CI == CL.Length)
            {
                GetWordError(WordMessagesError.EndString);
            }

            CI++;
            return TypeSymbol.U_Cst_Str;
        }

        internal static Dictionary<int, char> invValues = new Dictionary<int, char> 
        { 
            { 10, 'a' },
            { 11, 'b' },
            { 12, 'c' },
            { 13, 'd' },
            { 14, 'e' },
            { 15, 'f' },
        };

        internal static Dictionary<char, int> values = new Dictionary<char, int> 
        { 
            { 'a', 10 },
            { 'b', 11 },
            { 'c', 12 },
            { 'd', 13 },
            { 'e', 14 },
            { 'f', 15 },
        };

        private TypeSymbol HandleNumber()
        {
            G_curr_Num = 0;
            bool isReal = false;
            bool singlePoint = true;

            // if hexa deciaml
            if (CC == '0' && NextCCInLine && char.ToLower(NextCC) == 'x')
            {
                CI += 2;
                if ((!CCInLine) || !(char.IsNumber(CC) || values.ContainsKey(char.ToLower(CC))))
                {
                    MakeWordError(WordMessagesError.HexaDeciaml);
                }

                while (CCInLine && (char.IsNumber(CC) || values.ContainsKey(char.ToLower(CC))))
                {
                    if (char.IsNumber(CC))
                    {
                        G_curr_Num = G_curr_Num * 16 + char.GetNumericValue(CC);
                    }
                    else
                    {
                        G_curr_Num = G_curr_Num * 16 + values[CC];
                    }
                    CI++;
                }

                return TypeSymbol.U_Cst_Int;
            }
            else    // orginal number
            {
                while (CCInLine && char.IsNumber(CC))
                {
                    G_curr_Num = G_curr_Num * 10 + char.GetNumericValue(CC);
                    CI++;
                    singlePoint = false;
                }

                double dec = 0;
                if (CCInLine && CC == '.')
                {
                    isReal = true;
                    int p = 10;
                    dec = 0;
                    CI++;

                    while (CCInLine && char.IsNumber(CC))
                    {
                        dec = dec + char.GetNumericValue(CC) / p;
                        CI++;
                        p *= 10;
                        singlePoint = false;
                    }

                    G_curr_Num += dec;
                }
                dec = 0;
                if (singlePoint)
                {
                    GetWordError(WordMessagesError.SinglePoint);
                }
                int positive = 1;
                if (NextCCInLine && char.ToLower(CC) == 'e' && (NextCC == '+' || NextCC == '-' || char.IsNumber(NextCC)))
                {
                    if ((NextCC == '+' || NextCC == '-') && !((CI + 2 < CL.Length) && (char.IsNumber(CL[CI + 2]))))
                    {
                        GetWordError(WordMessagesError.NotFound, "digit");
                    }

                    if (NextCC == '+')
                    {
                        CI += 2;
                    }
                    else if (NextCC == '-')
                    {
                        CI += 2;
                        positive = -1;
                    }
                    else
                    {
                        CI++;
                    }
                    isReal = true;

                    while (CCInLine && char.IsNumber(CC))
                    {
                        dec = dec * 10 + char.GetNumericValue(CC);
                        CI++;
                    }
                }

                G_curr_Num = G_curr_Num * Math.Pow(10, dec * positive);
                if (double.IsInfinity(G_curr_Num))
                {
                    GetWordError(WordMessagesError.OverFlow);
                }

                if (isReal)
                {
                    return TypeSymbol.U_Cst_Real;
                }

                return TypeSymbol.U_Cst_Int;
            }
        }
    }
}