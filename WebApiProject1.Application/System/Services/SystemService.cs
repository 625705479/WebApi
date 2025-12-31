using Furion;
using Furion.DataEncryption;
using Furion.Localization;
using Furion.TaskScheduler;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;
using WebApiProject1.Application.Test;
using WebApiProject1.Application.Test.Dtos;
using WebApiProject1.Application.Test.Services;
using WebApiProject1.Application.UntinesHelper;
using WebApiProject1.Core;


namespace WebApiProject1.Application.System.Services
{
    public class SystemService : ISystemService, ITransient
    {
        public ResultData<object> DelEntryData(string tablename, string primaryKeyName, string KeyValue)
        {
            ResultData<object> resultData = new ResultData<object>();
            if (!string.IsNullOrEmpty(primaryKeyName))
            {

              SQLiteHelper.Instance.DelEntityData(
                  tableName: tablename,
             primaryKeyNameValue: KeyValue,  
              primaryKeyName: primaryKeyName,           
                  deleteTable: false              
                  );
               
            }
            else
            {
                Untines.SetError(resultData, EnumExtensions.MyErrorEnum.FailedToDeleteData);
            }
            return resultData;
        }

        public ResultData<object> GetEntryData(string EntryDataName)
        {
            ResultData<object> resultData = new ResultData<object>();
            if (!string.IsNullOrEmpty(EntryDataName))
            {
                var queryResult = SQLiteHelper.Instance.Query(EntryDataName).ToList();
                resultData.Data = queryResult;
            }
            else
            {
                Untines.SetError(resultData, EnumExtensions.MyErrorEnum.QueryError);
            }


            return resultData;


        }

        public ResultData<object> UpdateOrAddEntryData(string EntryDataName, string[] primaryKeyName, string[] KeyValue)
        {
            ResultData<object> resultData = new ResultData<object>();
            if (!string.IsNullOrEmpty(EntryDataName))
            {
               
            
                SQLiteHelper.Instance.ExecuteNonQuery($"INSERT INTO {EntryDataName}('{primaryKeyName}')  VALUES ('{KeyValue}')");


            }

            return resultData;
        }
    } 
}
