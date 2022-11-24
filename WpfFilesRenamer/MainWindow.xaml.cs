using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace WpfFilesRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            var directory = new DirectoryInfo(tbPath.Text);

            RenameFilesInDirectory(directory);

            RenameFilesInAllSubdirectories(directory);
        }

        private void RenameFilesInAllSubdirectories(DirectoryInfo directory)
        {
            foreach (var dir in directory.GetDirectories())
            {
                listBox.Items.Add("Directory: " + dir.Name);

                RenameFilesInDirectory(dir);
            }
        }

        private void RenameFilesInDirectory(DirectoryInfo directory)
        {
            var files = GetFiles(directory);

            if (files != null && files.Length > 0)
            {
                RenameFiles(directory.FullName, files);
            }
        }

        private string[] GetFiles(DirectoryInfo directory)
        {
            string[] files = directory.GetFiles().Select(f => f.Name).ToArray();     

            SortFilesByNumberInName(files);

            return files;
        }

        private void RenameFiles(string path, string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                var extention = Path.GetExtension(files[i]);
                var oldName = Path.GetFileName(files[i]);
                var newName = string.Format("{0}{1}{2}", tbNamePattern.Text, i + 1, extention);
                var oldPath = Path.Combine(path, oldName);
                var newPath = Path.Combine(path, newName);

                if (!File.Exists(newPath))
                {
                    File.Move(oldPath, newPath);
                }

                listBox.Items.Add($"{oldName} => {newName}");
            }
        }

        private void SortFilesByNumberInName(string[] filesNames)
        {
            Dictionary<int, string> filesDic = new Dictionary<int, string>();

            string[] sorted = new string[filesNames.Length];

            for (int i = 0; i < filesNames.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(filesNames[i]);
                var number = GetNumberFromString(fileName);

                filesDic[number] = filesNames[i];
            }

            var sortedKeys = filesDic.Keys.OrderBy(k => k);

            for (int i = 0; i < filesNames.Length; i++)
            {
                filesNames[i] = filesDic[sortedKeys.ElementAt(i)];
            }
        }

        private int GetNumberFromString(ReadOnlySpan<char> str)
        {
            int startIndex = str.Contains('_') ? str.IndexOf('_') : str.IndexOf('-');
            startIndex = startIndex > 0 ? startIndex + 1 : 0;
            var slice = str.Slice(startIndex, str.Length - startIndex);

            int x = 0;
            var digits = new char[slice.Length];

            for (int i = 0; i < slice.Length; i++)
            {
                if (Char.IsDigit(slice[i]))
                {
                    digits[x] = slice[i];
                    x++;
                }
            }

            return int.Parse(digits);
        }

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var ookiiDialog = new VistaFolderBrowserDialog();

            if (ookiiDialog.ShowDialog() == true)
            {
                tbPath.Text = ookiiDialog.SelectedPath;
            }
        }
    }
}
