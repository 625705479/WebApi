using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject1.Application.Test.Services;

namespace WebApiProject1.Application.Test
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [ApiDescriptionSettings("Default")]
    [ApiController]
    [Route("api/[controller]")]
    public class TestAppService: IDynamicApiController 
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
        public List<GradingDetail> GetGradingDetailAll()
        {
            var result = _testService.GetAllGradingDetailsAsync();
            return result;
        }
        /// <summary>
        /// 根据ID获取挡位信息
        /// </summary>
        [HttpGet("GetById")]
        public GradingDetail GetById(int id)
        {

            var result = _testService.GetGradingDetailByIdAsync(id);
            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpDelete("GetString")]
        public string GetString()
        {
            return _testService.GetString();
        }
    }
}
