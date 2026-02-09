using Furion;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace WebApiProject1.Core
{
    /// <summary>
    /// 数据库上下文对象
    /// </summary>
    public static class DbContext
    {
        /// <summary>
        /// SqlSugar 数据库实例
        /// </summary>
        public static readonly SqlSugarScope Instance = new(
            // 读取 appsettings.json 中的 ConnectionConfigs 配置节点
            App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs")
            , db =>
            {
                db.Ado.IsEnableLogEvent = true; //开启SQL执行日志
                // 这里配置全局事件，比如拦截执行 SQL
                // 添加记录SQL日志事件
                db.GetConnection("PostgreSQLDB").Aop.OnLogExecuting = (sql, pars) =>
                  {
                      // 记录SQL日志
                      sql = UtilMethods.GetSqlString(DbType.PostgreSQL, sql, pars);
                      Console.WriteLine(sql);
                      Logger.Info(sql);
                  };
                db.GetConnection("SqliteDB").Aop.OnLogExecuting = (sql, pars) =>
                {
                    // 记录SQL日志
                    sql = UtilMethods.GetSqlString(DbType.Sqlite, sql, pars);
                    Console.WriteLine(sql);
                    Logger.Info(sql);
                };
            });


    }
}
