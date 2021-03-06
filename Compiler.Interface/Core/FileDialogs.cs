﻿using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;

namespace Compiler.Interface
{
    public interface IFileDialogsService
    {
        MessageBoxResult ShowConfirmation(string content);

        string[] OpenFileDialog();

        string SaveFileDialog();
    }

    public class FileDialogsService : IFileDialogsService
    {
        public MessageBoxResult ShowConfirmation(string content)
        {
            return MessageBox.Show(content, "Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public string[] OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            Initialize(dialog);
            dialog.Multiselect = true;

            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                return dialog.FileNames;
            return null;
        }

        public string SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            Initialize(dialog);

            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
                return dialog.FileName;
            return null;
        }

        private void Initialize(FileDialog dialog)
        {
            dialog.DefaultExt = ".aub";
            dialog.Filter = "Compiler Files|*.aub";
        }
    }
}