using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Application.System.Services
{
    public interface ISystemService
    {
        //获取本机系统名称
        ResultData<object> GetDescription();
        //同步数据
        ResultData<object> SynchroData();
     

        }
}
