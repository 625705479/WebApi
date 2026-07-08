using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// 面包屑页面实体
    /// </summary>
    public class BreadItem
    {
        public string PageName { get; set; }
        public string PageTag { get; set; }
    }

    public partial class Window1 : Window
    {
        // 面包屑集合
        private ObservableCollection<BreadItem> _breadList = new ObservableCollection<BreadItem>();
        private BreadItem _currentSelectItem;

        public Window1()
        {
            InitializeComponent();
            icBreadCrumb.ItemsSource = _breadList;

            // 默认加载工作台，固定不可关闭
            AddBreadItem("工作台", "Work");
            frameMain.Navigate(new PageWork());
            rbWork.IsChecked = true;
        }

        #region 菜单切换逻辑
        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            string tag = radio.Tag.ToString();
            string pageName = "";

            switch (tag)
            {
                case "Work":
                    pageName = "工作台";
                    frameMain.Navigate(new PageWork());
                    break;
                case "System":
                    pageName = "系统管理";
                    frameMain.Navigate(new PageSystem());
                    break;
                case "Log":
                    pageName = "日志管理";
                    frameMain.Navigate(new PageLog());
                    break;
            }

            // 判断标签是否已存在，不存在则新增
            bool exist = false;
            foreach (var item in _breadList)
            {
                if (item.PageTag == tag)
                {
                    exist = true;
                    _currentSelectItem = item;
                    break;
                }
            }
            if (!exist)
            {
                AddBreadItem(pageName, tag);
            }
        }
        #endregion

        #region 面包屑操作
        /// <summary>
        /// 添加面包屑标签
        /// </summary>
        private void AddBreadItem(string pageName, string tag)
        {
            var newItem = new BreadItem() { PageName = pageName, PageTag = tag };
            _breadList.Add(newItem);
            _currentSelectItem = newItem;
        }

        /// <summary>
        /// 点击面包屑标签跳转页面
        /// </summary>
        private void BreadItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var stack = sender as StackPanel;
            var data = stack.DataContext as BreadItem;
            if (data == null) return;

            _currentSelectItem = data;
            switch (data.PageTag)
            {
                case "Work":
                    rbWork.IsChecked = true;
                    break;
                case "System":
                    rbSystem.IsChecked = true;
                    break;
                case "Log":
                    rbLog.IsChecked = true;
                    break;
            }
        }

        /// <summary>
        /// 关闭面包屑标签
        /// </summary>
        private void CloseBreadItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var stack = btn.Parent as StackPanel;
            var delItem = stack.DataContext as BreadItem;
            if (delItem == null || delItem.PageTag == "Work") return;

            // 删除当前标签
            int delIndex = _breadList.IndexOf(delItem);
            _breadList.Remove(delItem);

            // 关闭后跳转到前一个标签
            int targetIndex = Math.Max(0, delIndex - 1);
            var targetItem = _breadList[targetIndex];
            switch (targetItem.PageTag)
            {
                case "Work":
                    rbWork.IsChecked = true;
                    break;
                case "System":
                    rbSystem.IsChecked = true;
                    break;
                case "Log":
                    rbLog.IsChecked = true;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 退出登录
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWin = new MainWindow();
            Application.Current.MainWindow = loginWin;
            loginWin.Show();
            this.Close();
        }
    }
}