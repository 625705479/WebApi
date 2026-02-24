using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Quartz;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject1.Core;

namespace WebApiProject1.Application.UntinesHelper
{
    /// <summary>
    /// 自定义Quartz作业类（要执行的定时任务逻辑）
    /// 必须实现IJob接口
    /// </summary>
    public class JobFor5Seconds : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // ========== 你要执行的核心方法 ==========
             YourMethod(); // 调用自定义方法
            await Task.CompletedTask;
        }
        private static int _nameIndex = 0;

        /// <summary>
        /// 你自己的业务方法（每60秒执行一次）
        /// </summary>
        private void YourMethod()
        {
            // 这里写你要执行的逻辑，示例：输出当前时间
            string sql = "SELECT * from test";
            var db = DbContext.Instance.GetConnection("SqliteDB");
            var resdata = db.SqlQueryable<dynamic>(sql).ToList();
         
            // 方式A：直接通过动态属性访问（推荐）
            int currentIndex = _nameIndex % resdata.Count;
            string nameValue = resdata[currentIndex].Name?.ToString() ?? "未知名称";
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}"+ nameValue);
            _nameIndex++;
        }
    }


    public class JobFor10Seconds : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // ========== 你要执行的核心方法 ==========
            YourMethod(); // 调用自定义方法
            await Task.CompletedTask;
        }

        /// <summary>
        /// 你自己的业务方法（每5秒执行一次）
        /// </summary>
        private void YourMethod()
        {
            // 这里写你要执行的逻辑，示例：输出当前时间
            string sql = "SELECT * from test";
            var db = DbContext.Instance.GetConnection("SqliteDB");
            var resdata = db.SqlQueryable<object>(sql).ToPageList(1, 4);


            Console.WriteLine($"方法执行时间：{DateTime.Now:HH:mm:ss.fff}");
        }
    }
}
