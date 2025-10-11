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
            var db = DbContext.Instance.GetConnection("SqliteDB");
            db.Queryable<object>().AS("grading_detail").ToList();
            var res = Untines.GetNonNullableType(typeof(GradingDetail));
            var x = Untines.Create();
            ResultData<object> resultData = new();
            resultData.Data = x.Item1;
            return resultData;

        }


    }
}
