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
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, tbPath.Text);

            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                var extention = System.IO.Path.GetExtension(files[i]);
                var oldName = System.IO.Path.GetFileName(files[i]);
                var newName = string.Format("{0}{1}{2}", tbNamePattern.Text, i + 1, extention);
                var newPath = System.IO.Path.Combine(path, newName);

                System.IO.File.Move(files[i], newPath);

                listBox.Items.Add($"{oldName} => {newName}");
            }
        }
    }
}
