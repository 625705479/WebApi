using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfApp1
{
    public partial class ForgetPwdWindow : Window
    {
        private DispatcherTimer _codeTimer;
        private int _countDown = 60;

        public ForgetPwdWindow()
        {
            InitializeComponent();
            // 初始化倒计时定时器
            _codeTimer = new DispatcherTimer();
            _codeTimer.Interval = TimeSpan.FromSeconds(1);
            _codeTimer.Tick += CodeTimer_Tick;
        }

        // 清空用户名
        private void ClearUserName(object sender, RoutedEventArgs e)
        {
            txtUserName.Clear();
        }

        // 获取验证码倒计时
        private void GetVerifyCode(object sender, RoutedEventArgs e)
        {
            string phone = txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(phone) || phone.Length != 11)
            {
                MessageBox.Show("请输入正确11位手机号", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // 此处调用发送短信接口
            MessageBox.Show("验证码已发送至您的手机", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            btnCode.IsEnabled = false;
            _countDown = 60;
            btnCode.Content = $"{_countDown}s后重发";
            _codeTimer.Start();
        }

        // 倒计时每秒刷新
        private void CodeTimer_Tick(object sender, EventArgs e)
        {
            _countDown--;
            btnCode.Content = $"{_countDown}s后重发";
            if (_countDown <= 0)
            {
                _codeTimer.Stop();
                btnCode.IsEnabled = true;
                btnCode.Content = "获取验证码";
            }
        }

        // 重置密码提交
        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string code = txtCode.Text.Trim();
            string newPwd = pwdNew.Password;
            string confirmPwd = pwdConfirmNew.Password;

            // 基础校验
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("请输入用户名", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(phone) || phone.Length != 11)
            {
                MessageBox.Show("请输入正确手机号", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("请输入短信验证码", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPwd.Length < 6)
            {
                MessageBox.Show("新密码至少6位字符", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newPwd != confirmPwd)
            {
                MessageBox.Show("两次输入新密码不一致", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 此处对接后端重置密码接口
            MessageBox.Show("密码重置成功，请使用新密码登录！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            BackToLogin(null, null);
        }

        // 返回登录窗口
        private void BackToLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow loginWin = new MainWindow();
            loginWin.Show();
            this.Close();
        }
    }
}