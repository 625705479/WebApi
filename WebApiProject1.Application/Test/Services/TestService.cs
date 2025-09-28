using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject1.Core;

namespace WebApiProject1.Application.Test.Services
{
    public class TestService : ITestService, ITransient
    {
        public string GetString()
        {
          return "Hello World";
        }

        List<GradingDetail> ITestService.GetAllGradingDetailsAsync()
        {
            var db = DbContext.Instance.GetConnection("PostgreSQLDB");

            // SqlSugar查询：查询grading_detail表，映射为GradingDetail实体
            return db.Queryable<GradingDetail>()
                              .AS("grading_detail") // 指定表名和别名
                              .ToList();


        }

        GradingDetail ITestService.GetGradingDetailByIdAsync(int id)
        {
            var db = DbContext.Instance.GetConnection("PostgreSQLDB");
            // 按ID查询单条记录
            return db.Queryable<GradingDetail>()
                           .AS("grading_detail", "o")
                           .Where(g => g.id == id)
                           .First();
        }
    }
}
