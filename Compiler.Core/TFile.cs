namespace Compiler.Core
{
    internal class TFile
    {
        internal TFile Next { get; set; }
        internal string Name { get; set; }

        internal static void AddFile(string name, TFile GFile)
        {
            TFile temp = GFile;
            TFile last = null;

            while (temp != null)
            {
                if (temp.Name == name)
                {
                    return;
                }
                last = temp;
                temp = temp.Next;
            }

            last.Next = new TFile { Name = name };
        }
    }
}