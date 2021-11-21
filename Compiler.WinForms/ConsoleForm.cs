namespace Compiler.Core;

public partial class ConsoleForm : Form
{
    public string ValueEntered { get; set; }

    public ConsoleForm()
    {
        InitializeComponent();
    }

    public void ClearScreen() => richBxConsole.Text = "";

    public void Write(string data, bool isLine) => richBxConsole.Text += data + ((isLine) ? "\n" : "");

    private void richBxConsole_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Close();
        }
    }
}
