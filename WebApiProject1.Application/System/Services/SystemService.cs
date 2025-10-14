using Furion;
using Furion.DataEncryption;
using Furion.Localization;
using Furion.TaskScheduler;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;
using WebApiProject1.Application.Test;
using WebApiProject1.Application.Test.Dtos;
using WebApiProject1.Application.UntinesHelper;
using WebApiProject1.Core;


namespace WebApiProject1.Application.System.Services
{
    public class SystemService : ISystemService, ITransient
    {
        public ResultData<object> GetDescription()
        {
            ResultData<object> resultData = new();
            var x = Untines.Create();
            resultData.Data = x.Item1;
            return resultData;
        }

        public  ResultData<object> SynchroData()
        {
            ResultData<object> resultData = new(); var db = DbContext.Instance.GetConnection("PostgreSQLDB");
            var queryResult = db.Queryable<GradingDetail>()
                                   .AS("grading_detail").ToList();
            var dto = queryResult.Adapt<List<GradingDetail>>();
            resultData.Data = dto;
            return resultData;
        }

      
    }
}
