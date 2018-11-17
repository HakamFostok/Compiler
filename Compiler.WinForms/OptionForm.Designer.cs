namespace Compiler.Core
{
    partial class OptionForm
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
            this.chkbxReload = new System.Windows.Forms.CheckBox();
            this.btnFont = new System.Windows.Forms.Button();
            this.fontDialogText = new System.Windows.Forms.FontDialog();
            this.grpBxOptions = new System.Windows.Forms.GroupBox();
            this.chkAutomaticSave = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkbxReload
            // 
            this.chkbxReload.AutoSize = true;
            this.chkbxReload.Location = new System.Drawing.Point(15, 23);
            this.chkbxReload.Name = "chkbxReload";
            this.chkbxReload.Size = new System.Drawing.Size(252, 17);
            this.chkbxReload.TabIndex = 0;
            this.chkbxReload.Text = "Reload all the files that opend in the last session";
            this.chkbxReload.UseVisualStyleBackColor = true;
            // 
            // btnFont
            // 
            this.btnFont.Location = new System.Drawing.Point(7, 69);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(75, 23);
            this.btnFont.TabIndex = 1;
            this.btnFont.Text = "Font";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // grpBxOptions
            // 
            this.grpBxOptions.Controls.Add(this.chkAutomaticSave);
            this.grpBxOptions.Controls.Add(this.chkbxReload);
            this.grpBxOptions.Controls.Add(this.btnFont);
            this.grpBxOptions.Location = new System.Drawing.Point(12, 12);
            this.grpBxOptions.Name = "grpBxOptions";
            this.grpBxOptions.Size = new System.Drawing.Size(273, 103);
            this.grpBxOptions.TabIndex = 2;
            this.grpBxOptions.TabStop = false;
            this.grpBxOptions.Text = "Options";
            // 
            // chkAutomaticSave
            // 
            this.chkAutomaticSave.AutoSize = true;
            this.chkAutomaticSave.Location = new System.Drawing.Point(15, 46);
            this.chkAutomaticSave.Name = "chkAutomaticSave";
            this.chkAutomaticSave.Size = new System.Drawing.Size(240, 17);
            this.chkAutomaticSave.TabIndex = 5;
            this.chkAutomaticSave.Text = "Automatic save the file when exit the program";
            this.chkAutomaticSave.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(15, 123);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(96, 123);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // OptionForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(293, 158);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.grpBxOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Option";
            this.grpBxOptions.ResumeLayout(false);
            this.grpBxOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkbxReload;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.FontDialog fontDialogText;
        private System.Windows.Forms.GroupBox grpBxOptions;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkAutomaticSave;
    }
}