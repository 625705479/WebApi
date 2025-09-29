using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject1.Application.Test.Dtos;

namespace WebApiProject1.Application.Test.Services
{
    public interface ITestService
    {
        /// <summary>
        /// 获取所有挡位信息
        /// </summary>
        /// <returns>挡位信息列表</returns>
        List<GradingDetail> GetAllGradingDetailsAsync(GradingQueryDetail gradingQuery);

        /// <summary>
        /// 根据ID查询挡位信息
        /// </summary>
        /// <param name="id">挡位ID</param>
        /// <returns>单个挡位信息</returns>
        GradingDetail GetGradingDetailByIdAsync(int id);
        string GetString();
    }

   
}
