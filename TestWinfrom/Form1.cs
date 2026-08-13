using MailKit.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace TestWinfrom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeAutoLayoutComponent();


            //RsaSignHelper.GenerateRsaKeyPair(out string pri, out string pub);
            //string machineCode = HardwareHelper.GetMachineCode();
            //// 10分钟测试
            //DateTime expire = DateTime.Now.AddMinutes(100);
            //string privateKey = @"<RSAKeyValue><Modulus>4bKHRo2yC/BbYwa+UoDueB81Cywy1dO/rQrLbfzkXP4K2W7GDIO7XMs24mk7SY0osVFZnQA+qFau5TKF2MCsf8fyyOJxblDxi4pqa3nQwM4jCeLdvZL7Qj+YBXKfNjkRJGsJJi2YAkI2M1yOc48q5CWAEftz3Jq/mcghZOoOe3Y4okeuioSy3MrnCadK+cQX3Fkrhklpjf14s6LRErCp2C8gA0On/9X9RO7peCoJ8udCwlxHrpIxrh6eDGLoDwiaLIUYA72QTcYAwMOAk+0Ir6ihnz0b3AYFErVw1/8CaCQfdWs2wYS41zWkZIG7stG0vjGnBwftKje6uwj+fBtTKQ==</Modulus><Exponent>AQAB</Exponent><P>8ztq5xLXg3AHzyuv+O19C0cjX0L1weaJkImmTD5oDUJfEcCTw1a1L0VuQ9ZS35Rq9RTht7r/1cmPq7/7GXpyz5Hb8sUPGoYHvj1LocDcTHxqpbRjdSbg6GgjWS3WLzyEHLM/TG2WAOd8mX5p9vYq3Nc4U2jVnl/VJkMq0U7G0+s=</P><Q>7Yt6IwPpgtzT6WoIYTxk2QyBTSFJbS4HbPAb+UJGQzZ2aEtMvOyrkIneVkLwpoSpE5seB8B0vSp7DHfThnqF7S9eQSBMmbB8ilmlRZah3y/DI9Kj3CwlrWHimTCLULLy9kgiCIo+yZudX8QOb57aT8SnkZMknJqFaPi5Y15GdDs=</Q><DP>E/8yjsTRyxCO0813rjN4MFEs60wKAGL/tE5cya/nxg9K2Z7Hhyu9waEnq5QXRCJjmLqaxAwvtFfZ4/jon/OdNMt9Fbx1vWx/fnhzm1zLv84KxozKEHudyf2lylMmZMPI6MMj1Ri9WF2vtL7b313lsDpReyoHRfoDAB5Nit+7IPk=</DP><DQ>Wdxhn81jELYpFCugb+hA3jr0zxDAjiTTekp6yphfrB12PY3+wZlmbY86JLe+AcA9lcUgXx5XCxh+5ACQbFb9QvSgW1K0p480DcJL2z9YjO2sjGiqxCePOT/GUN0kVqrbbn9rIH/rsKjFp+yq6V7Wh0aFfXSEbRmnTkaJGyYW0PE=</DQ><InverseQ>5hF6lYLpzpKcDSjwyzrtmZPqMlci53UoXo3yTModUqrimPV30pevKFQRpSSCmc2oeC+D1aZH2aSJk5YlWYIer7xT5aZI3vAyLAma8ptPNyFqAMU1NndlscujqCPr8ho314+6mlk3F7waXdgPZFJiG1Zb7H9pWu3sc7rSOYopnVY=</InverseQ><D>rmtAnaatXQqgJVQ1yx62rAA6ButeUd81du4rrlFMzgzJp6UyysMXDaxCOxDl7352XyomHe3tfjyXJqs3wv2LkaidGN/el0lYkeUjPvHCAO6NJ3u6r2GiaV0qB7PAFLBfbgyF4opDuiMfLewubmHK3MuaQMtZi7fPsHF4VTuIe6G6r+F4g7pJZmaKOLAETDNcbh5HTbAn5mSOinjcg7brCIQIfbmUINFd8MXBnu54IONbMott+kq+usDb0b/SRMnT7LHQvyYTZ6mzQCbvo0Nt692ImJUhc2QrxTlSfXvbM5VQWhO++RXMEhXb2A7sR34kCpmyYsZv2QuMLNLqXuhUXQ==</D></RSAKeyValue>";
            //string authCode = LicenseManager.GenerateLicenseCode(machineCode, expire, privateKey, "测试授权");
            //Console.WriteLine("激活码：" + authCode);


            //LicenseManager.SaveLicense(authCode);
            //MessageBox.Show("激活成功，请重启软件");
            SendEamil();
        }

        public async void SendEamil() {


            // 初始化多条SMTP通道
            var channelRoot = ConfigLoader.LoadEmailChannels();
            var channels = channelRoot.Channels;
            // 传入通道管理器
            var channelManager = new SmtpChannelManager(channels);
            // 收件人163邮箱，自动匹配163通道
            string toMail = "16639421145@163.com";
            var selectConfig = channelManager.GetAutoChannel(toMail);
            var sender = new EmailSender(selectConfig);
            var attachList = new List<string>()
{
    @"E:\backfile\2026-04-11_20260730162810947.csv"
};
            var model = new Dictionary<string, object>()
{
    {"Title","舔狗日记"},
    {"UserName","宝宝"},
    {"Content","其实我早就感觉到了，你对我从来没有多余的热情。\r\n所有热情都是我单方面撑起的，聊天永远是我开启，话题永远靠我寻找，我的碎碎念，在你那里只是无关紧要的消息。\r\n我一次次说服自己再坚持一下，或许再努力一点，结局就会不一样。慢慢我才懂，不喜欢就是不喜欢，再多主动和付出，也换不来对等的在意。\r\n我攒了无数次勇气来找你，最后只剩下满满的失望。我可以继续喜欢你，但我好像，再也没有力气主动了。"},
    {"Link",""},
    {"SendTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
};
            await sender.SendTemplateMailAsync(toMail, "系统消息", "NotifyEmail.vm", model, attachList);





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
