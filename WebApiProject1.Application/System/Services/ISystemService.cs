using static WebApiProject1.Application.SystemAppService;

namespace WebApiProject1.Application
{
    public interface ISystemService
    {
        List<object> GetDescription();
    }
}
