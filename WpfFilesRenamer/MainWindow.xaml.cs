using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            //var path = System.IO.Path.Combine(Environment.CurrentDirectory, tbPath.Text);
            var path = tbPath.Text;

            var directory = new DirectoryInfo(path);

            string[] files = directory.GetFiles().Select(f => f.Name).ToArray();

            SortFilesNames(files);

            for (int i = 0; i < files.Length; i++)
            {
                var extention = System.IO.Path.GetExtension(files[i]);
                var oldName = System.IO.Path.GetFileName(files[i]);
                var newName = string.Format("{0}{1}{2}", tbNamePattern.Text, i + 1, extention);
                var oldPath = System.IO.Path.Combine(path, oldName);
                var newPath = System.IO.Path.Combine(path, newName);

                if (!File.Exists(newPath))
                {
                    System.IO.File.Move(oldPath, newPath);
                }

                listBox.Items.Add($"{oldName} => {newName}");
            }
        }

        private void SortFilesNames(string[] filesNames)
        {
            Dictionary<int, string> filesDic = new Dictionary<int, string>();

            string[] sorted = new string[filesNames.Length];

            for (int i = 0; i < filesNames.Length; i++)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(filesNames[i]);
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
