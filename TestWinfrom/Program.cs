using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinfrom
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //bool pass = LicenseManager.CheckLicense(out string tip);
            //if (!pass)
            //{
            //    MessageBox.Show(tip, "授权校验失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return; // 直接退出，禁止进入软件
            //}
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    
}
