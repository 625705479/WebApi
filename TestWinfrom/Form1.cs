using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinfrom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeAutoLayoutComponent();
        }
        /// <summary>
        /// 自动构建自适应布局，无需拖拽控件，代码一键生成
        /// </summary>
        private void InitializeAutoLayoutComponent()
        {
            // 窗口基础配置
            Text = "BCT/氧碳日志解析工具";
            Size = new Size(700, 300);
            MinimumSize = new Size(600, 300);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("微软雅黑", 9f);

         
            TableLayoutPanel tableMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 1
            };
            // 左列固定宽度，右列自动填充
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(tableMain);

            #region 左侧容器（固定420宽）
            Panel panelLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0) };
            tableMain.Controls.Add(panelLeft, 0, 0);

            // 1. 文件目录行
            TableLayoutPanel tableDir = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = new Padding(0, 0, 0, 12),
                ColumnCount = 3,
                RowCount = 2
            };
            tableDir.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableDir.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableDir.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // 第一行：源目录
            Label lblSrc = new Label
            {
                Text = "选择文件目录：",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 26
            };
            TextBox txtSrc = new TextBox { Dock = DockStyle.Fill, Height = 26 };
            Button btnSelSrc = new Button { Text = "选择目录", Height = 26 };
            tableDir.Controls.Add(lblSrc, 0, 0);
            tableDir.Controls.Add(txtSrc, 1, 0);
            tableDir.Controls.Add(btnSelSrc, 2, 0);

            // 第二行：输出目录
            Label lblOut = new Label
            {
                Text = "输出目录：",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 26
            };
            TextBox txtOut = new TextBox { Dock = DockStyle.Fill, Height = 26 };
            Button btnSelOut = new Button { Text = "选择输出目录", Height = 26 };
            tableDir.Controls.Add(lblOut, 0, 1);
            tableDir.Controls.Add(txtOut, 1, 1);
            tableDir.Controls.Add(btnSelOut, 2, 1);
            panelLeft.Controls.Add(tableDir);

            // 2. 功能按钮组
            TableLayoutPanel tableBtn = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3,
                Margin = new Padding(0, 0, 0, 12)
            };
            tableBtn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableBtn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableBtn.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            Button btnStart = new Button { Text = "开启定时任务", Dock = DockStyle.Fill, Height = 40 };
            Button btnStop = new Button { Text = "停止定时任务", Dock = DockStyle.Fill, Height = 40 };
            Button btnClear = new Button
            {
                Text = "清理文件",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(220, 240, 255)
            };
            tableBtn.Controls.Add(btnStart, 0, 0);
            tableBtn.Controls.Add(btnStop, 1, 0);
            tableBtn.Controls.Add(btnClear, 2, 0);
            panelLeft.Controls.Add(tableBtn);

    
            DataGridView dgvLog = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvLog.Columns.Add("colTime", "LocalTime");
            dgvLog.Columns.Add("colLevel", "LogLevel");
            dgvLog.Columns.Add("colMsg", "Message");
            dgvLog.Columns["colTime"].FillWeight = 2;
            dgvLog.Columns["colLevel"].FillWeight = 1;
            dgvLog.Columns["colMsg"].FillWeight = 7;
            panelLeft.Controls.Add(dgvLog);
            #endregion

            #region 右侧参数面板（自适应宽度）
            Panel panelRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 0, 0, 0) };
            tableMain.Controls.Add(panelRight, 1, 0);

            TableLayoutPanel tableConfig = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 5
            };
            tableConfig.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var configItems = new (string LabelText, int Row)[]
            {
                ("氧碳设备ID：", 0),
                ("Bct车间：", 1),
                ("Bct设备号：", 2),
                ("Bct最大数据量：", 3),
                ("氧碳最大数量：", 4)
            };

            foreach (var item in configItems)
            {
                Label lbl = new Label
                {
                    Text = item.LabelText,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Height = 28,
                    Margin = new Padding(0, 6, 8, 0)
                };
                TextBox txt = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Height = 28,
                    Margin = new Padding(0, 6, 0, 0)
                };
                tableConfig.Controls.Add(lbl, 0, item.Row);
                tableConfig.Controls.Add(txt, 1, item.Row);
            }
            panelRight.Controls.Add(tableConfig);
            #endregion
        }

    }
}
