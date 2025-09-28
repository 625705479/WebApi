using Microsoft.IdentityModel.Logging;
using WebApiProject1.Core;

namespace WebApiProject1.Application
{
    public class SystemService : ISystemService, ITransient
    {
        public List<object> GetDescription()
        {
            var  db = DbContext.Instance.GetConnection("SqliteDB");
          var result= db.Queryable<object>().AS("grading_record_log", "o").ToList();
            return result;
            
        }
    }
}
