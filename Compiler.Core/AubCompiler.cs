using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Compiler.Core
{
    /// <summary>
    /// Represent the compiler that will provide service to compile files in specific langauge.
    /// </summary>
    public class AubCompiler
    {

#if MyCode
        private Dictionary<string, TypeSymbol> Keywords { get; set; }
#else
        internal static TSymbol Gsymbol { get; private set; }
#endif
        private static BinaryFormatter formatter { get; set; }
        public List<WarningException> Warning { get; private set; }
        private SyntaxAnalyst analyst { get; set; }
        private Variables locals { get; set; }
        public string dir { get; set; }

        private TFile gFile
        {
            get
            {
                return locals.Gfile;
            }
            set
            {
                locals.Gfile = value;
            }
        }

        private TFile currentFile
        {
            get
            {
                return locals.CurrFile;
            }
            set
            {
                locals.CurrFile = value;
            }
        }

        private TProcedure gProc
        {
            get
            {
                return locals.GPorc;
            }
        }

        /// <summary>
        /// Initialize a new instance of Compiler Class.
        /// </summary>
        public AubCompiler()
        {
            locals = new Variables();
            Warning = new List<WarningException>();
            analyst = new SyntaxAnalyst(locals, Warning);
        }

        static AubCompiler()
        {
            formatter = new BinaryFormatter();
            ReadKeyWords();
        }

        /// <summary>
        /// Reads the keywords from file.
        /// </summary>
        private static void ReadKeyWords()
        {

#if MyCode

            Keywords = File.ReadAllLines(Compiler.CoreResource.KeyWordsFile).
                            Select(str => str.Trim()).
                            ToDictionary(str => str, str => (TypeSymbol)Enum.Parse(typeof(TypeSymbol), str, false));

#elif ModifiedCode

            keywordsSymbol = File.ReadAllLines(Compiler.CoreResource.KeyWordsFile).Select(str => str.Trim()).
                Select(str => new Symbol { Name = str, Kind = (TypeSymbol)Enum.Parse(typeof(TypeSymbol), str, false) }).
                Reverse().Aggregate((one, two) =>
                {
                    two.Next = one;
                    return two;
                });
            
            // this is the old way using modified structure (IEnumerable Symbol) sequence
 
            //List<Symbol> list = File.ReadAllLines(Compiler.CoreResource.KeyWordsFile).Select(str => str.Trim()).
            //    Select(str => new Symbol { Name = str, Kind = (TypeSymbol)Enum.Parse(typeof(TypeSymbol), str, false) }).ToList();
            //keywordsSymbol = list.First();
            //Symbol last = list.Aggregate((one, two) => one.Next = two);
#else
            TSymbol last = null;
            int counter = 0;

            using (StreamReader reader = File.OpenText(CompilerLibraryResource.KeyWordsFile))
            {
                while (!reader.EndOfStream)
                {
                    string key = reader.ReadLine().Trim();
                    TSymbol symbol = new TSymbol() { Name = key, Next = null, UL = (TypeSymbol)counter };
                    counter++;

                    if (Gsymbol == null)
                    {
                        Gsymbol = symbol;
                    }
                    else
                    {
                        last.Next = symbol;
                    }

                    last = symbol;
                }
            }
#endif
        }

        /// <summary>
        /// Compile the primary file that has main function and the related included file with it.
        /// </summary>
        /// <param name="fileName">Represent the filename</param>
        public void CompileMainProgram(string fileName)
        {
            Warning.Clear();

            locals.initializeForRecompile();
            gFile = new TFile { Name = fileName };
            currentFile = gFile;
            bool mainFile = true;

            while (currentFile != null)
            {
                locals.initializeForNewFile();
                try
                {
                    locals.CF = File.ReadAllLines(currentFile.Name);
                }
                catch (FileNotFoundException)   // if the file that included is not found.
                {
                    MakeSyntaxError(SyntaxMessagesError.FileNotFound);
                }
                catch (DirectoryNotFoundException)
                {
                    MakeSyntaxError(SyntaxMessagesError.FileNotFound);
                }

                analyst.CompileCurrentFile();
                if (mainFile)       // if there is NO main is not in main File.
                {
                    if (TIdentifier.FindIdentifer("Main", gProc) == null)
                    {
                        MakeSyntaxError(string.Format(SyntaxMessagesError.NoMainMethod, currentFile.Name));
                    }
                }

                mainFile = false;
                currentFile = currentFile.Next;
            }

            // Check if we call function or procedure that not Defined
            TProcedure tempP = gProc;
            while (tempP != null)
            {
                if (!tempP.IsDefined)
                {
                    throw new SyntaxErrorException(string.Format(SyntaxMessagesError.MethodNotDefined, tempP.Name), 0, 0, "");
                    //MakeSyntaxError(string.Format(SyntaxMessagesError.MethodNotDefined, tempP.Name));
                }
                tempP = (TProcedure)tempP.Next;
            }

            string[] dirs = gFile.Name.Split('\\');
            dir = dirs.Take(dirs.Length - 1).Select(str => str + "\\").Aggregate((one,two) => one + two) + dirs.Last().Split('.')[0] + ".obj";
            using (FileStream writer = new FileStream(dir, FileMode.Create))
            {
                formatter.Serialize(writer, gProc);
            }
        }

        private void MakeSyntaxError(string message)
        {
            throw new SyntaxErrorException(message, 0, 0, currentFile.Name);
        }
    }   // end class Compiler
}   // end namespace Compiler Library