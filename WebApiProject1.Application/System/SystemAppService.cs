using WebApiProject1.Application.System.Services;
using WebApiProject1.Application.UntinesHelper;
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
        public ResultData<object> GetEntryData(string EntryDataName) { 
        
           var result=    _systemService.GetEntryData(EntryDataName);
       
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

            var result = _systemService.DelEntryData(EntryDataName, primaryKeyName,  KeyValue);

            return result;

        }
        [HttpPost("UpdateOrAddEntryData")]
        public ResultData<object> UpdateOrAddEntryData(string EntryDataName, string primaryKeyName, string KeyValue)
        {

            string[] xx = new string[] { primaryKeyName };
            string[] yy = new string[] { KeyValue };
         
        
            var result = _systemService.UpdateOrAddEntryData(EntryDataName, xx, yy);

            return result;

        }






    }


}
