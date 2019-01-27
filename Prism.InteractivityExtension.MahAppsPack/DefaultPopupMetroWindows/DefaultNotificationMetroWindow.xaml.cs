using MahApps.Metro.Controls;
using Prism.Interactivity.InteractionRequest;
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
using System.Windows.Shapes;

namespace Prism.InteractivityExtension.MahAppsPack.DefaultPopupMetroWindows
{
    /// <summary>
    /// DefaultNotificationMetroWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DefaultNotificationMetroWindow : MetroWindow
    {
        public DefaultNotificationMetroWindow()
        {
            InitializeComponent();
        }

        public INotification Notification
        {
            get
            {
                return this.DataContext as INotification;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
