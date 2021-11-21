namespace Compiler.Core;

[System.Serializable]
abstract class IdentifierInstruction
{
    internal IdentifierInstruction Next { get; set; }
    internal string Name { get; set; }
    internal TypeSymbol UL { get; set; }
    internal static T FindIdentifer<T>(string name, T varibale) where T : IdentifierInstruction
    {
        T temp = varibale;
        while (temp != null)
        {
            if (temp.Name.ToUpper() == name.ToUpper())
            {
                return temp;
            }
            temp = (T)temp.Next;
        }

        return null;
    }

    internal static void AddIdentifier<T>(string name, ref T gid) where T : IdentifierInstruction, new()
    {
        T temp = new() { Name = name, UL = TypeSymbol.U_UnKown };
        temp.Next = (T)gid;
        gid = temp;
    }
}
