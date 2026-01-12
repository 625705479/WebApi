using MathNet.Numerics.Distributions;
using System.Data;
using WebApiProject1.Application.System.Services;
using WebApiProject1.Application.Test;
using WebApiProject1.Application.UntinesHelper;
using WebApiProject1.Core;
namespace WebApiProject1.Application.System
{
    /// <summary>
    /// 系统服务接口
    /// </summary>
    [ApiDescriptionSettings("Success")]
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAppService : IDynamicApiController
    {
        private readonly ISystemService _systemService;
        public SystemAppService(ISystemService systemService)
        {
            _systemService = systemService;
        }
        /// <summary>
        /// 根据实体名称获取数据
        /// </summary>
        /// <param name="EntryDataName"></param>
        /// <returns></returns>
        [HttpGet("GetEntryData")]
        public ResultData<object> GetEntryData(string EntryDataName)
        {

            var result = _systemService.GetEntryData(EntryDataName);

            return result;

        }
        /// <summary>
        /// 根据实体名称删除
        /// </summary>
        /// <param name="EntryDataName">实体名称</param>
        /// <param name="primaryKeyName">主键或者条件</param>
        /// <param name="KeyValue">值</param>
        /// <returns></returns>
        [HttpPost("DelEntryData")]
        public ResultData<object> DelEntryData(string EntryDataName, string primaryKeyName, string KeyValue)
        {

            var result = _systemService.DelEntryData(EntryDataName, primaryKeyName, KeyValue);

            return result;

        }
        /// <summary>
        /// 根据实体名称修改
        /// </summary>
        /// <param name="EntryDataName">实体名称</param>
        /// <param name="primaryKeyName">要修改的字段</param>
        /// <param name="KeyValue">修改的值</param>
        /// <param name="WhereValue">修改的值</param>
        /// <returns></returns>
        [HttpPost("UpdateOrAddEntryData")]
        public ResultData<object> UpdateOrAddEntryData(string EntryDataName, string primaryKeyName, string KeyValue, string WhereValue)
        {
            var db = DbContext.Instance.GetConnection("SqliteDB");
            string[] xx = new string[] { primaryKeyName };
            string[] yy = new string[] { KeyValue };
            List<string> ints = db.Queryable<GradingDetail>().AS("grading_detail").Select(s => s.grading_position).ToList();
            DataTable list = db.Queryable<GradingDetail>().AS("grading_detail").Select(it => new { id = it.id, name = it.grading_position }).ToDataTable();//2个字段 
            var res = db.Utilities.DataTableToList<GradingDetail>(list);
            var result = _systemService.UpdateOrAddEntryData(EntryDataName, xx, yy, WhereValue);
            var item = new TestEntity() { Name = "测试数据", Age = 13 };
            var x = db.Storageable(item).ToStorage();
            var res1 = x.AsInsertable.ExecuteCommand();//不存在插入
            var res2 = x.AsUpdateable.ExecuteCommand();//存在更新
            var dt = db.Ado.GetDataTable("SELECT * FROM  grading_detail");

            var dalist = db.SqlQueryable<object>("select * from test").ToList();
            dalist.ForEach(item =>
            {
                dynamic dynamicItem = item;
                string name = dynamicItem.Name;
                var age = dynamicItem.Age;
                Console.WriteLine($"遍历结果：{name} {age}");
            });
            //更新
            db.Updateable<object>()
                   .AS("test")
                   .SetColumns("Name", "管理员")
                   .SetColumns("Age", 32)
                   .Where("Id=0").ExecuteCommand();



            return result;

        }
        [SugarTable("test")]
        public class TestEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            public string Name { get; set; }

            public int Age { get; set; }
        }





    }


}
