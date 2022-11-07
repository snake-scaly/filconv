using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;

namespace FilConvWpf
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            version.Text = string.Format("{0}.{1}.{2}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(((Hyperlink)sender).NavigateUri.ToString());
        }
    }
}
