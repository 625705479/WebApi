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

        string GetString();

        bool CreateOrSaveFile(string ThingxmlPath, string RemoteThingPath, string ThingTemplatespPath, string ExcelPath, string originalNumber, string RepaceNumber);
        ResultData<object> InsertOrUpdateDataAsync(TestTable test);

        ResultData<object> DeleteDataAsync(int id);

        ResultData<object> GetresultData();

        ResultData<object> GetResult();

    }


}
