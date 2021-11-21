
using Compiler.Core;
using Compiler.WinForms.Properties;

namespace Compiler.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        try
        {
            myCompiler = new AubCompiler();
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show("Critical Error occur\n" +
                "File \"keywords\" is not found" +
                "The application will terminate.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.Exit();
        }

        try
        {
            exe = new Executer();

            Action<object, WriteEventArgs> writeHandler = (obj, eve) => consoleInputOutput.Write(eve.Line, eve.IsLine);

            exe.EndOfExecute += new EventHandler<WriteEventArgs>(writeHandler);
            exe.WriteEvent += new EventHandler<WriteEventArgs>(writeHandler);

            consoleInputOutput = new ConsoleForm();
            programSettings = new Settings();
            if (programSettings.ReloadFiles)
            {
                try
                {
                    string[] filesName = programSettings.OpenFiles.Split('\n');
                    foreach (string name in filesName)
                    {
                        OpenFile(name);
                    }
                }
                catch (ArgumentException)
                {
                    ForceEnableDisable(false);
                }
            }
            else
            {
                ForceEnableDisable(false);
            }
        }
        catch (FileNotFoundException)
        {
        }
    }

    private bool compileSuccess { get; set; }
    private ConsoleForm consoleInputOutput { get; set; }
    private AboutForm about { get; set; }
    private OptionForm options { get; set; }
    private Settings programSettings { get; set; }
    private AubCompiler myCompiler { get; set; }
    private Executer exe { get; set; }

    private string StatuBar
    {
        set => toolStripStatusLabel1.Text = value;
    }

    private string[] OpenedFiles
    {
        get
        {
            try
            {
                return mainTabControl.TabPages.Cast<TabPage>().Select(page => Convert.ToString(page.Tag)).ToArray();
            }
            catch (InvalidOperationException)
            {
                return new string[0];
            }
        }
    }

    private TabPage SelectedPage => mainTabControl.SelectedTab;

    private RichTextBox SelectedRichTextBox => (RichTextBox)SelectedPage.Controls[Properties.Resources.RichTextBoxControl];

    private string[] CurrentText
    {
        get => SelectedRichTextBox.Lines;
        set => SelectedRichTextBox.Lines = value;
    }

    #region File tool strip event Handler

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            using (File.Create(saveFile.FileName))
            {
                InitializeTabPageForFile(saveFile.FileName, false);
            }
        }
    }

    private void openToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            OpenFile(openFile.FileName);
        }
    }

    private void InitializeTabPageForFile(string fileName, bool open)
    {
        RichTextBox ri = new() { Dock = DockStyle.Fill, Name = Properties.Resources.RichTextBoxControl, Font = programSettings.TextFont };
        ri.TextChanged += (o, e) => StatuBar = "Ready";
        TabPage page = new(new DirectoryInfo(fileName).Name) { Tag = fileName };
        page.Controls.Add(ri);
        mainTabControl.TabPages.Add(page);
        if (open)
        {
            ri.Lines = File.ReadAllLines(fileName);
        }
        page.Controls[Properties.Resources.RichTextBoxControl].Select();
    }

    private void OpenFile(string fileName) => InitializeTabPageForFile(fileName, true);

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DialogResult result = AskForSave();
        CloseTabPage(SelectedPage, result);
    }

    private void closeAllToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        DialogResult result = AskForSave();

        foreach (TabPage page in mainTabControl.TabPages)
        {
            CloseTabPage(page, result);
        }
    }

    private DialogResult AskForSave() => MessageBox.Show("Do you want to save changes to the files?", "Save",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

    private void CloseTabPage(TabPage page, DialogResult result)
    {
        if (result == System.Windows.Forms.DialogResult.Yes)
        {
            SaveCodeToFile(page);
            mainTabControl.TabPages.Remove(page);
        }
        else if (result == System.Windows.Forms.DialogResult.No)
        {
            mainTabControl.TabPages.Remove(page);
        }
    }

    private void SaveCodeToFile(TabPage page) => File.WriteAllLines((string)page.Tag, ((RichTextBox)page.Controls[Resources.RichTextBoxControl]).Lines);

    private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveCodeToFile(SelectedPage);

    private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        foreach (TabPage page in mainTabControl.TabPages)
        {
            SaveCodeToFile(page);
        }
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {

        }
    }

    private void printToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {

        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

    #endregion

    #region Edit tool strip event Handler

    private void redoToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Redo();

    private void undoToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Undo();

    private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.SelectAll();

    private void deSelectAllToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.DeselectAll();

    private void clearToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Clear();

    private void cutToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Cut();

    private void copyToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Copy();

    private void pastToolStripMenuItem_Click(object sender, EventArgs e) => SelectedRichTextBox.Paste();

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (options == null)
        {
            options = new OptionForm
            {
                ReloadFiles = programSettings.ReloadFiles,
                newFont = programSettings.TextFont,
                AutoSave = programSettings.AutoSave
            };
        }

        if (options.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            if (options.FontChnagedFlag)
            {
                programSettings.TextFont = options.newFont;
            }
            programSettings.AutoSave = options.AutoSave;
            programSettings.ReloadFiles = options.ReloadFiles;
        }
    }

    #endregion

    #region Debug tool strip event Handler

    private void buildToolStripMenuItem_Click(object sender, EventArgs e)
    {
        saveAllToolStripMenuItem_Click(saveAllToolStripMenuItem, new EventArgs());
        lstbxErrorList.Items.Clear();
        if (mainTabControl.TabCount > 0)
        {
            try
            {
                myCompiler.CompileMainProgram(Convert.ToString(SelectedPage.Tag));
            }
            catch (CompileTimeErrorException exc)
            {
                compileSuccess = false;
                string kindOfError;
                if (exc is MacroErrorException)
                {
                    kindOfError = "Macro Error";
                }
                else if (exc is LexicalErrorException)
                {
                    kindOfError = "Lexical Error";
                }
                else
                {
                    kindOfError = "Syntax Error";
                }
                //lstbxErrorList.Items.Add(exc.Message + " " + exc.Column + " " + exc.LineNumber + " " + exc.FileName + " " + kindOfError);
                lstbxErrorList.Items.Add("Message     : " + exc.Message);
                lstbxErrorList.Items.Add("Column      : " + exc.Column);
                lstbxErrorList.Items.Add("Line Number  : " + exc.LineNumber);
                lstbxErrorList.Items.Add("File Name    : " + exc.FileName);
                lstbxErrorList.Items.Add("Error Kind   : " + kindOfError);
                lstbxErrorList.Items.Add("");
                //int start = CurrentText.Take(exc.LineNumber - 1).Select(str => str.Length).Sum() + exc.Column;
                //SelectedRichTextBox.Select(start, 1);
                StatuBar = "Compile Failed";
                return;
            }
            finally
            {
                foreach (WarningException warn in myCompiler.Warning)
                {
                    lstbxErrorList.Items.Add("Message     : " + warn.Message);
                    lstbxErrorList.Items.Add("Column      : " + warn.Column);
                    lstbxErrorList.Items.Add("Line Number  : " + warn.LineNumber);
                    lstbxErrorList.Items.Add("File Name    : " + warn.FileName);
                    lstbxErrorList.Items.Add("Error Kind : Warnning");
                    lstbxErrorList.Items.Add("------------------------------------------");
                }
            }
            compileSuccess = true;
            StatuBar = "Compile Success";
        }
    }

    private void executeFromObjectFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (openFileDialogObj.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Execute(openFileDialogObj.FileName);
        }
    }

    private void executeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        buildToolStripMenuItem_Click(buildToolStripMenuItem, new EventArgs());
        if (compileSuccess)
        {
            Execute(myCompiler.dir);
        }
        else
        {
            StatuBar = "Execute Fail Because of Compile Time Errors";
        }
    }

    private void Execute(string path)
    {
        try
        {
            consoleInputOutput = new ConsoleForm();
            consoleInputOutput.Show();
            exe.ExecuteMainMethodFromObjectFile(path);
        }
        catch (RuntimeErrorException error)
        {
            MessageBox.Show(error.Message, "Runtime Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            consoleInputOutput.Close();
        }
    }

    #endregion

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (about == null)
        {
            about = new AboutForm();
        }

        about.ShowDialog();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (programSettings.AutoSave)
        {
            saveAllToolStripMenuItem_Click(saveAllToolStripMenuItem, new EventArgs());
        }
        try
        {
            programSettings.OpenFiles = mainTabControl.TabPages.Cast<TabPage>()
                .Select(page => Convert.ToString(page.Tag)).Aggregate((one, two) => one + '\n' + two);
        }
        catch (InvalidOperationException)
        {
            programSettings.OpenFiles = "";
        }
        finally
        {
            programSettings.Save();
        }
    }

    private void mainTabControl_ControlAdded(object sender, ControlEventArgs e) => EnableOrDisableBuildAndExecute(true);

    private void mainTabControl_ControlRemoved(object sender, ControlEventArgs e) => EnableOrDisableBuildAndExecute(false);

    private void EnableOrDisableBuildAndExecute(bool enable)
    {
        if (mainTabControl.TabCount == 1)
        {
            ForceEnableDisable(enable);
        }
    }

    private void ForceEnableDisable(bool enable)
    {
        executeToolStripMenuItem.Enabled = enable;
        buildToolStripMenuItem.Enabled = enable;

        executeToolStripMenuItem.Visible = enable;
        buildToolStripMenuItem.Visible = enable;

        debugToolStripMenuItem.Visible = enable;
    }

}
