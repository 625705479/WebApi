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
        /// <param name="grading_position">挡位名称</param>
        /// <param name="item">料号</param>
        /// <param name="Pagenumber">当前页码</param>
        /// <param name="PageSize">查询记录条数</param>
        /// <returns></returns>
        [HttpPost("GetGradingDetailAll")] // 显式指定路由为 GetGradingDetailAll
        [HttpGet("GetGradingDetailAll")] 
        public ResultData<object> GetGradingDetailAll(string grading_position, string item, int Pagenumber = 1, int PageSize = 20)
        {
            var gradingQuery = new GradingQueryDetail
            {
                GradingDetail = new grading_detail { grading_position = grading_position, item = item },
                PageSize = PageSize,
                PageNumber = Pagenumber
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
        [HttpGet("GetString")]
        public ResultData<object> GetString()
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
        public ResultData<object> InsertOrUpdateDataAsync(int id, string name, int age)
        {
            TestTable testTable = new TestTable
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
        [HttpDelete("DeleteDataAsync")]
        public ResultData<object> DeleteDataAsync(int id)
        {

            var result = _testService.DeleteDataAsync(id);
            return result;
        }

        public ResultData<object> GetresultData()
        {
            var result = _testService.GetresultData();
            return result;
        }

        public ResultData<object> Getresult()
        {
            var result = _testService.GetResult();

            return result;
        }
        /// <summary>
        /// 获取定时任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("StartJob")]
        public Task< ResultData<object>> StartJob()
        {
            var result = _testService.StartJob();

            return result;
        }

        /// <summary>
        /// 停止定时任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("StopJob")]
        public Task<ResultData<object>> StopJob()
        {
            var result = _testService.StopJob();

            return result;
        }

        public ResultData<object> GetDoubleIntimacy(string fristname, string secondname)
        {
            var result = _testService.GetDoubleIntimacy(fristname,secondname);

            return result;
        }
        /// <summary>发起请假申请</summary>
        /// <param name="applyUserId">申请人ID</param>
        /// <param name="leaveDays">请假天数</param>
        /// <param name="leaveReason">请假原因</param>
        [HttpPost("StartLeave")]
        public async Task<ResultData<object>> StartLeave(string applyUserId, int leaveDays, string leaveReason)
        {
            var res = new ResultData<object>
            {
                // 关键：实例化 BaseResponse 对象
                BaseResponse = new BaseResponse()
            };
            try
            {
                var instanceId = await _testService.StartLeaveAsync(applyUserId, leaveDays, leaveReason);
                res.BaseResponse.Message = $"已发起请假，流程ID：{instanceId}请假人:{applyUserId},天数:{leaveDays},原由:{leaveReason}";
                res.Data = new { InstanceId = instanceId };
            }
            catch (Exception ex)
            {
                res.BaseResponse.ChineseError = ex.Message;
            }
            return res;
        }
        /// <summary>经理审批</summary>
        [HttpPost("ManagerAudit")]
        public async Task<ResultData<object>> ManagerAudit(string ApplyUserId, string instanceId, bool isAgree, string comment = "")
        {
            var res = new ResultData<object>
            {
                BaseResponse = new BaseResponse()
            };
            var ok = await _testService.ManagerAuditAsync(ApplyUserId, instanceId, isAgree, comment);
            res.Data = new { Success = ok };
            res.BaseResponse.Message = ok ? "操作成功" : "操作失败，当前流程不允许经理审批";
            return res;
        }

        /// <summary>总监审批</summary>
        [HttpPost("DirectorAudit")]
        public async Task<ResultData<object>> DirectorAudit(string ApplyUserId, string instanceId, bool isAgree, string comment = "")
        {
            var res = new ResultData<object>
            {
                BaseResponse = new BaseResponse()
            };
            var ok = await _testService.DirectorAuditAsync(ApplyUserId, instanceId, isAgree, comment);
            res.Data = new { Success = ok };
            res.BaseResponse.Message = ok ? "操作成功" : "操作失败，必须等待经理审批完成后才可操作";
            return res;
        }

        /// <summary>总经理审批</summary>
        [HttpPost("GeneralManagerAudit")]
        public async Task<ResultData<object>> GeneralManagerAudit(string ApplyUserId, string instanceId, bool isAgree, string comment = "")
        {
            var res = new ResultData<object>
            {
                BaseResponse = new BaseResponse()
            };
            var ok = await _testService.GeneralManagerAuditAsync(ApplyUserId, instanceId, isAgree, comment);
            res.Data = new { Success = ok };
            res.BaseResponse.Message = ok ? "操作成功" : "操作失败，必须等待总监审批完成后才可操作";
            return res;
        }

        /// <summary>查询流程详情</summary>
        [HttpGet("GetFlow")]
        public async Task<ResultData<object>> GetFlow(string instanceId)
        {
            var res = new ResultData<object>();
            res.Data = await _testService.GetFlowAsync(instanceId);
            return res;
        }
        /// <summary>取消流程</summary>
        [HttpGet("CancelFlow")]
        public async Task<ResultData<object>> CancelFlow(string ApplyUserId, string instanceId)
        {
            var res = new ResultData<object>
            {
                // 关键：实例化 BaseResponse 对象
                BaseResponse = new BaseResponse()
            };
            res.BaseResponse.Message = $"流程ID：{instanceId}已经被取消，请重新申请";
            res.Data = await _testService.CancelFlowAsync(ApplyUserId,instanceId);
            return res;
        }
        /// <summary>获取全部流程列表</summary>
        [HttpGet("GetAll")]
        public async Task<ResultData<object>> GetAll()
        {
            var res = new ResultData<object>();
            res.Data = await _testService.GetAllFlowAsync();
            return res;
        }
    }



}
