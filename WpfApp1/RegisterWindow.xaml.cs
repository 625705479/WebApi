using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        // 清空用户名输入框
        private void ClearUserName(object sender, RoutedEventArgs e)
        {
            txtUserName.Clear();
        }

        // 跳转登录窗口
        private void ToLoginWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.txtUserName.Text = txtUserName.Text;
            login.Show();
            this.Close();
        }

        // 注册提交逻辑
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string pwd = pwdPassword.Password;
            string confirmPwd = pwdConfirm.Password;
            string phone = txtPhone.Text.Trim();

            // 基础校验
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("请输入用户名", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(pwd) || pwd.Length < 6)
            {
                MessageBox.Show("密码至少6位字符", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (pwd != confirmPwd)
            {
                MessageBox.Show("两次输入密码不一致", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!cbAgree.IsChecked.Value)
            {
                MessageBox.Show("请勾选同意用户协议", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 此处对接注册接口/数据库保存账号
            MessageBox.Show("注册成功，请前往登录！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            // 注册成功跳转登录
            ToLoginWindow(null, null);
        }
    }
}