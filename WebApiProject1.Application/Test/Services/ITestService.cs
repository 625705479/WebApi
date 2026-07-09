using WebApiProject1.Application.Test.Dtos;
using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Application.Test.Services
{
    public interface ITestService
    {
        /// <summary>
        /// 获取所有挡位信息
        /// </summary>
        /// <returns>挡位信息列表</returns>
        ResultData<object> GetAllGradingDetailsAsync(GradingQueryDetail gradingQuery);

        /// <summary>
        /// 根据ID查询挡位信息
        /// </summary>
        /// <param name="id">挡位ID</param>
        /// <returns>单个挡位信息</returns>
        ResultData<object> GetGradingDetailByIdAsync(int id);

        ResultData<object> GetString();

        bool CreateOrSaveFile(string ThingxmlPath, string RemoteThingPath, string ThingTemplatespPath, string ExcelPath, string originalNumber, string RepaceNumber);
        ResultData<object> InsertOrUpdateDataAsync(TestTable test);

        ResultData<object> DeleteDataAsync(int id);

        ResultData<object> GetresultData();

        ResultData<object> GetResult();
        Task<ResultData<object>> StopJob();
        ResultData<object> GetDoubleIntimacy(string fristname, string secondname);
        Task<ResultData<object>> StartJob();

        /// <summary>发起请假申请，返回流程实例ID</summary>
        Task<string> StartLeaveAsync(string applyUserId, int leaveDays, string leaveReason);

        /// <summary>经理审批</summary>
        Task<bool> ManagerAuditAsync(string instanceId, bool isAgree, string comment = "");

        /// <summary>总监审批</summary>
        Task<bool> DirectorAuditAsync(string instanceId, bool isAgree, string comment = "");
        /// <summary>
        /// 总经理审批
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="isAgree"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<bool> GeneralManagerAuditAsync(string instanceId, bool isAgree, string comment = "");


        /// <summary>获取流程详情</summary>
        Task<LeaveFlowInstance> GetFlowAsync(string instanceId);

        /// <summary>获取全部流程列表</summary>
        Task<List<LeaveFlowInstance>> GetAllFlowAsync();

        /// <summary>取消流程（仅未结束可取消）</summary>
        Task<bool> CancelFlowAsync(string instanceId);
    }


}
