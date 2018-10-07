using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace ToDoList.Views
{
    /// <summary>
    /// FileView.xaml の相互作用ロジック
    /// </summary>
    public partial class FileView : UserControl
    {
        public FileView()
        {
            InitializeComponent();
        }

        private void ExportFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.Filter = "xlsx 形式|*.xlsx|All Files (*.*)|*.*";
            saveFileDialog.FileName = "ToDoListMetro_Export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                this.ExportPathAndFileName.Text = saveFileDialog.FileName;
            }
            BindingOperations.GetBindingExpression(this.ExportPathAndFileName, TextBox.TextProperty).UpdateSource();
        }

        private void ImportFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 1;
            openFileDialog.Filter = "xlsx 形式|*.xlsx|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.ImportPathAndFileName.Text = openFileDialog.FileName;
            }
            BindingOperations.GetBindingExpression(this.ImportPathAndFileName, TextBox.TextProperty).UpdateSource();
        }

        private void BackupFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.Filter = "xlsx 形式|*.xlsx|All Files (*.*)|*.*";
            saveFileDialog.FileName = "ToDoListMetro_Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                this.BackupPathAndFileName.Text = saveFileDialog.FileName;
            }
            BindingOperations.GetBindingExpression(this.BackupPathAndFileName, TextBox.TextProperty).UpdateSource();
        }

        private void RestoreFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 1;
            openFileDialog.Filter = "xlsx 形式|*.xlsx|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.RestorePathAndFileName.Text = openFileDialog.FileName;
            }
             BindingOperations.GetBindingExpression(this.RestorePathAndFileName, TextBox.TextProperty).UpdateSource();
       }
    }
}
