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
                SortFilesByNumberInName(files);

                RenameFiles(directory.FullName, files);
            }
        }

        private string[] GetFiles(DirectoryInfo directory)
        {
            return directory.GetFiles().Select(f => f.Name).ToArray();
        }

        private void RenameFiles(string path, string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {

                var (oldName, oldPath, newName, newPath) = GetPathsAndNames(files[i], i, path);

                if (!File.Exists(newPath))
                {
                    File.Move(oldPath, newPath);
                }

                listBox.Items.Add($"{oldName} => {newName}");
            }
        }

        private (string oldName, string oldPath, string newName, string newPath) GetPathsAndNames(string file, int number, string path)
        {
            var extension = Path.GetExtension(file);
            var oldName = Path.GetFileName(file);
            var newName = string.Format("{0}{1}{2}", tbNamePattern.Text, number + 1, extension);
            var oldPath = Path.Combine(path, oldName);
            var newPath = Path.Combine(path, newName);

            return (oldName, oldPath, newName, newPath);
        }

        private void SortFilesByNumberInName(string[] filesNames)
        {
            Dictionary<int, string> filesDic = new Dictionary<int, string>();

            string[] sorted = new string[filesNames.Length];

            for (int i = 0; i < filesNames.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(filesNames[i]);
                var number = GetNumberFromFileName(fileName);

                filesDic[number] = filesNames[i];
            }

            var sortedKeys = filesDic.Keys.OrderBy(k => k);

            for (int i = 0; i < filesNames.Length; i++)
            {
                filesNames[i] = filesDic[sortedKeys.ElementAt(i)];
            }
        }

        private int GetNumberFromFileName(ReadOnlySpan<char> fileName)
        {
            var partWithNumber = GetNamePartWithNumber(fileName);
            
            return GetNumberFromString(partWithNumber);
        }

        private ReadOnlySpan<char> GetNamePartWithNumber(ReadOnlySpan<char> fileName)
        {
            const char hyphen = '-';
            const char underscore = '_';

            int startIndex = 0;

            if (fileName.Contains(hyphen)) startIndex = fileName.IndexOf(hyphen);
            if (fileName.Contains(underscore)) startIndex = fileName.IndexOf(underscore);

            if (startIndex != 0 && ((startIndex + 1) < fileName.Length)) startIndex++;

            return fileName.Slice(startIndex, fileName.Length - startIndex);
        }

        private static int GetNumberFromString(ReadOnlySpan<char> str)
        {
            if (str.Length == 0) return 0;

            int j = 0;
            char[] digits = new char[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsDigit(str[i]))
                {
                    digits[j] = str[i];
                    j++;
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
