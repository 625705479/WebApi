using WebApiProject1.Application.System.Services;
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
        /// 获取验证码
        /// </summary>
        /// <returns></returns>

        public string GetDescription()
        {



            return _systemService.GetDescription();

        }





    }


}
