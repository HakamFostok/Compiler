namespace Compiler.Core;

public partial class InputForm : Form
{
    public string Input { get; private set; }

    public InputForm()
    {
        InitializeComponent();
    }

    private void txtbxInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Input = txtbxInput.Text;
            Close();
        }
    }

    private void InputForm_Load(object sender, System.EventArgs e) => txtbxInput.Text = "";

}
