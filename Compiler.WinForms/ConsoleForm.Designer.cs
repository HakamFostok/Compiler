namespace Compiler.Core
{
    partial class ConsoleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richBxConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richBxConsole
            // 
            this.richBxConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.richBxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richBxConsole.Font = new System.Drawing.Font("Tahoma", 14F);
            this.richBxConsole.ForeColor = System.Drawing.Color.White;
            this.richBxConsole.Location = new System.Drawing.Point(0, 0);
            this.richBxConsole.Name = "richBxConsole";
            this.richBxConsole.ReadOnly = true;
            this.richBxConsole.Size = new System.Drawing.Size(705, 580);
            this.richBxConsole.TabIndex = 0;
            this.richBxConsole.Text = "";
            this.richBxConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richBxConsole_KeyDown);
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 580);
            this.Controls.Add(this.richBxConsole);
            this.MaximizeBox = false;
            this.Name = "ConsoleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Console";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richBxConsole;
    }
}