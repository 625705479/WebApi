using Furion.DynamicApiController;
using Microsoft.OpenApi.Validations.Rules;
namespace WebApiProject1.Application
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
        /// 获取系统描述
        /// </summary>
        /// <returns></returns>

        public List<object> GetDescription()
        {
            return _systemService.GetDescription();
        }

  
    }
}
