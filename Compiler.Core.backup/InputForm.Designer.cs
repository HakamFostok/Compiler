namespace Compiler.Core
{
    partial class InputForm
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
            this.txtbxInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpbxInput = new System.Windows.Forms.GroupBox();
            this.grpbxInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtbxInput
            // 
            this.txtbxInput.Location = new System.Drawing.Point(52, 26);
            this.txtbxInput.Name = "txtbxInput";
            this.txtbxInput.Size = new System.Drawing.Size(360, 20);
            this.txtbxInput.TabIndex = 0;
            this.txtbxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbxInput_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input :";
            // 
            // grpbxInput
            // 
            this.grpbxInput.Controls.Add(this.label1);
            this.grpbxInput.Controls.Add(this.txtbxInput);
            this.grpbxInput.Location = new System.Drawing.Point(12, 12);
            this.grpbxInput.Name = "grpbxInput";
            this.grpbxInput.Size = new System.Drawing.Size(427, 65);
            this.grpbxInput.TabIndex = 2;
            this.grpbxInput.TabStop = false;
            this.grpbxInput.Text = "Inputs";
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 93);
            this.Controls.Add(this.grpbxInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputForm";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.grpbxInput.ResumeLayout(false);
            this.grpbxInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtbxInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpbxInput;
    }
}