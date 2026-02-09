using WebApiProject1.Application.UntinesHelper;


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

        public ResultData<object> UpdateOrAddEntryData(string EntryDataName, string[] primaryKeyName, string[] KeyValue, string WhereValue)
        {
            ResultData<object> resultData = new ResultData<object>();
            if (!string.IsNullOrEmpty(EntryDataName) && string.IsNullOrEmpty(WhereValue))
            {
                string primaryKeyNames = primaryKeyName[0];
                string KeyValues = KeyValue[0];


                SQLiteHelper.Instance.ExecuteNonQuery($"INSERT INTO {EntryDataName}({primaryKeyName[0]})  VALUES ({KeyValue[0]})");


            }
            else if (!string.IsNullOrEmpty(EntryDataName) && !string.IsNullOrEmpty(WhereValue))
            {
                SQLiteHelper.Instance.ExecuteNonQuery($"UPDATE  {EntryDataName} SET('{primaryKeyName[0]}')  VALUES ('{KeyValue[0]}')");
            }

            return resultData;
        }
    }
}
