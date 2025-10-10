using WebApiProject1.Application.Test;
using WebApiProject1.Core;

namespace WebApiProject1.Application.System.Services
{
    public class SystemService : ISystemService, ITransient
    {
        public string GetDescription()
        {
            var db = DbContext.Instance.GetConnection("SqliteDB");
            db.Queryable<object>().AS("grading_detail").ToList();
            var res = Untines.GetNonNullableType(typeof(GradingDetail));
            var x = Untines.Create();

            return x.Item1;

        }


    }
}
