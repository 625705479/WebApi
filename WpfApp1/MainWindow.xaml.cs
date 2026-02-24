using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
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
        // 清空用户名输入框
        private void BtnClearUser_Click(object sender, RoutedEventArgs e)
        {
            txtUserName.Clear();
        }

        // 登录按钮核心逻辑：转圈加载、账号校验、窗口淡入淡出切换
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUserName.Text.Trim();
            string pwd = pwdBox.Password;

            // 非空校验
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("请输入用户名和密码", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 获取按钮内两个状态面板
            var template = btnLogin.Template;
            var panelNormal = template.FindName("panelNormal", btnLogin) as StackPanel;
            var panelLoading = template.FindName("panelLoading", btnLogin) as StackPanel;

            // 切换加载状态，禁用按钮防止重复点击
            panelNormal.Visibility = Visibility.Collapsed;
            panelLoading.Visibility = Visibility.Visible;
            btnLogin.IsEnabled = false;

            // 模拟接口网络请求延时
            await Task.Delay(1500);

            // 登录校验 测试账号 admin / 123456
            if (user != "admin" || pwd != "123456")
            {
                MessageBox.Show("用户名或密码错误", "登录失败", MessageBoxButton.OK, MessageBoxImage.Error);
                // 恢复按钮正常状态
                panelNormal.Visibility = Visibility.Visible;
                panelLoading.Visibility = Visibility.Collapsed;
                btnLogin.IsEnabled = true;
                return;
            }

            // 登录成功：当前登录窗口淡出动画
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, ev) =>
            {
                // 实例化主业务窗口
                Window1 mainWin = new Window1();
                mainWin.Opacity = 0;
                mainWin.Show();
                // 主窗口淡入动画
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                mainWin.BeginAnimation(Window.OpacityProperty, fadeIn);
                // 关闭登录窗口
                this.Close();
            };
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        // 忘记密码弹窗提示
        private void TxtForgetPwd_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("跳转密码找回页面", "功能提示");
        }

        // 注册账号弹窗提示
        private void TxtRegister_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("跳转用户注册页面", "功能提示");
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

