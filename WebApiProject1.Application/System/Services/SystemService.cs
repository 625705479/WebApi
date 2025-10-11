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


    }
}
