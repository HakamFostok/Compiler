﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Compiler.Core
{
    public partial class OptionForm : Form
    {
        internal bool FontChnagedFlag { get; set; }
        internal Font newFont { get; set; }
        internal bool ReloadFiles
        {
            get
            {
                return chkbxReload.Checked;
            }
            set
            {
                chkbxReload.Checked = value;
            }
        }

        internal bool AutoSave
        {
            get
            {
                return chkAutomaticSave.Checked;
            }
            set
            {
                chkAutomaticSave.Checked = value;
            }
        }

        public OptionForm()
        {
            InitializeComponent();
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            if (fontDialogText.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontChnagedFlag = true;
                newFont = fontDialogText.Font;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void CloseForm()
        {
            Close();
        }


    }
}
