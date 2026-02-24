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

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            frameMain.Navigate(new PageWork());
            rbWork.IsChecked = true;
        }
        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            string tag = radio.Tag.ToString();
            switch (tag)
            {
                case "Work":
                    txtPageTitle.Text = "工作台";
                    frameMain.Navigate(new PageWork());
                    break;
                case "System":
                    txtPageTitle.Text = "系统管理";
                    frameMain.Navigate(new PageSystem());
                    break;
                case "Log":
                    txtPageTitle.Text = "日志管理";
                    frameMain.Navigate(new PageLog());
                    break;
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWin = new MainWindow();
            Application.Current.MainWindow = loginWin;
            loginWin.Show();
            this.Close();
        }
    }
}

