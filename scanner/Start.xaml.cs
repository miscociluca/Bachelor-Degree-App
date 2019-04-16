using ImageCropper;
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

namespace scanner
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        public Start()
        {
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri(@"C:\Users\misco\source\repos\scanner\scanner\Resources\main.jpg")));
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            main.Owner = this;
            main.Show();
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            Admin adm = new Admin();
            adm.WindowStartupLocation= WindowStartupLocation.CenterScreen;
            adm.Owner = this;
            adm.Show();
        }
    }
}
