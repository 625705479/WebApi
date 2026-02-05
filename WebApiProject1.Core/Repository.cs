using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject1.Core
{

    public class Repository<T> : SimpleClient<T> where T : class, new()
    {
        public static ISqlSugarClient db1 = DbContext.Instance.GetConnection("PostgreSQLDB");
        public Repository()
        {
            ISqlSugarClient db = db1;
            base.Context = db;
        }
    }
    }
