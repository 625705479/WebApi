using System.ComponentModel;
using WebApiProject1.Application.Test.Dtos;
using WebApiProject1.Application.Test.Services;
using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Application.Test
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [ApiDescriptionSettings("Default")]
    [ApiController]
    [Route("api/[controller]")]
    public class TestAppService : IDynamicApiController
    {
        private readonly ITestService _testService;
        public TestAppService(ITestService testService)
        {
            _testService = testService;
        }
        /// <summary>
        /// 获取挡位信息
        /// </summary>
        [HttpPost("GetGradingDetailAll")] // 显式指定路由为 GetGradingDetailAll
        public ResultData<object> GetGradingDetailAll(string grading_position, string item)
        {
            GradingQueryDetail gradingQuery = new GradingQueryDetail
            {
                grading_position = grading_position,
                item = item
            };
            var result = _testService.GetAllGradingDetailsAsync(gradingQuery);
            return result;
        }
        /// <summary>
        /// 根据ID获取挡位信息
        /// </summary>
        [HttpGet("GetById")]
        public ResultData<object> GetById(int id)
        {

            var result = _testService.GetGradingDetailByIdAsync(id);
            return result;

        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <returns></returns>
        [HttpDelete("GetString")]
        public string GetString()
        {
            return _testService.GetString();
        }

        /// <summary>
        /// 创建thing xml文件和remote thing xml文件
        /// </summary>
        /// <param name="ThingxmlPath">Thingxml路径</param>
        /// <param name="RemoteThingPath">RemoteThing路径</param>
        /// <param name="ThingTemplatespPath">ThingTemplates路径</param>
        /// <param name="ExcelPath">ExcelPath路径</param>
        /// <param name="originalNumber">原始目标数字</param>
        /// <param name="RepaceNumber">替换的数字</param>
        /// <returns></returns>
        [HttpPost("CreateOrSaveFile")]
        public bool CreateOrSaveFile(string ThingxmlPath, string RemoteThingPath, string ThingTemplatespPath, string ExcelPath, string originalNumber, string RepaceNumber)
        {
            return _testService.CreateOrSaveFile(ThingxmlPath, RemoteThingPath, ThingTemplatespPath, ExcelPath, originalNumber, RepaceNumber);
        }
        /// <summary>
        /// 添加或者修改数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        [HttpGet("InsertDataAsync")]
        public ResultData<object> InsertOrUpdateDataAsync(int id,string name,int age)
        {
            TestTable  testTable = new TestTable
            {
                Id = id,
                Name = name,
                Age = age
            };
            var result = _testService.InsertOrUpdateDataAsync(testTable);
            return result;
        }
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteDataAsync")]
        public ResultData<object> DeleteDataAsync(int id)
        {
    
            var result = _testService.DeleteDataAsync(id);
            return result;
        }
    }
}
