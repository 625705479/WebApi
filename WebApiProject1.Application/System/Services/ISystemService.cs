using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Application.System.Services
{
    public interface ISystemService
    {
        ResultData<object> GetEntryData(string EntryDataName);
        ResultData<object> DelEntryData(string EntryDataName, string primaryKeyName, string KeyValue);
        ResultData<object> UpdateOrAddEntryData(string EntryDataName, string[] primaryKeyName, string[] KeyValue);




    }
}
