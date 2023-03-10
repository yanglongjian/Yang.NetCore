using Furion;
using Furion.DependencyInjection;
using Furion.Logging.Extensions;
using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Yang.Core
{
    /// <summary>
    /// SqlSugar上下文配置
    /// </summary>
    public class DbContext : IScoped
    {
        /// <summary>
        /// 用来处理事务多表查询和复杂的操作
        /// </summary>
        public SqlSugarClient Db;
        public DbContext()
        {
            ICacheService cacheService = new HttpRuntimeCache();
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = App.Configuration["ConnectionConfig:ConnectionString"],
                DbType = (DbType)App.Configuration["ConnectionConfig:DbType"].ToInt(),
                IsAutoCloseConnection = App.Configuration["ConnectionConfig:IsAutoCloseConnection"].ToBool(),
                InitKeyType = InitKeyType.Attribute,//从特性读取主键自增信息
                LanguageType = LanguageType.Chinese,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    DataInfoCacheService = cacheService, //配置我们创建的缓存类
                    EntityService = (c, p) =>
                    {
                        // int?  decimal?这种 isnullable=true
                        if (c.PropertyType.IsGenericType &&
                        c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }
                        else if (c.PropertyType == typeof(string) && c.GetCustomAttribute<RequiredAttribute>() == null)
                        {
                            //string类型如果没有Required isnullable=true
                            p.IsNullable = true;
                        }

                        if ((DbType)App.Configuration["ConnectionConfig:DbType"].ToInt() == DbType.SqlServer)
                        {
                            if (!(p.DataType ?? "").Contains("nvarchar"))
                            {

                                if (p.DataType == "longtext" && c.PropertyType == typeof(string))
                                {
                                    p.DataType = "text";
                                }

                                if (p.DataType != "text" && c.PropertyType == typeof(string))
                                {
                                    p.DataType = "nvarchar(200)";
                                }
                            }
                        }
                    }
                },

            });

            #region 数据审计

            Db.Aop.DataExecuting = (oldValue, entity) =>
            {
                //插入或者更新可以修改 实体里面的值，比如插入或者更新 赋默认值 (审计)
                if (entity.OperationType == DataFilterType.InsertByObject)
                {
                    switch (entity.PropertyName)
                    {
                        case "CreatedTime":
                            entity.SetValue(DateTime.Now); break;
                        case "CreatedId":
                            var userId = App.User?.FindFirstValue("userId");
                            entity.SetValue(userId); break;
                        default:
                            break;
                    }
                }
                else if (entity.OperationType == DataFilterType.UpdateByObject)
                {
                    switch (entity.PropertyName)
                    {
                        case "UpdatedTime":
                            entity.SetValue(DateTime.Now); break;
                        case "UpdatedId":
                            var userId = App.User?.FindFirstValue("userId");
                            entity.SetValue(userId); break;
                        default:
                            break;
                    }
                }
            };

            #endregion

            #region  异常日志打印

            Db.Aop.OnError = (exp) =>
            {
                $"sql执行异常\r\n:{exp.Sql}".LogError();
            };

            #endregion

            #region Sql执行耗时记录
            Db.Aop.OnLogExecuted = (sql, p) =>
            {
                if (App.Configuration["ConnectionConfig:IsLogTimeoutSql"].ToBool())
                {
                    //执行时间超过
                    if (Db.Ado.SqlExecutionTime.TotalSeconds > App.Configuration["ConnectionConfig:TimeoutSeconds"].ToInt())
                    {
                        $"执行sql缓慢[{Db.Ado.SqlExecutionTime.TotalSeconds}]S : {sql} {p.ToJson()}".LogWarning();
                    }
                }
            };

            #endregion

            #region 数据审计日志

            Db.Aop.OnDiffLogEvent = it =>
            {
                //数据审计需要在ExecuteCommand之前执行 EnableDiffLogEvent()

                string tableName = string.Empty;
                string tableDescription = string.Empty;
                string beforeData = string.Empty;
                string afterData = string.Empty;

                switch (it.DiffType)
                {
                    case DiffType.insert:
                        tableName = it.AfterData?.FirstOrDefault()?.TableName;
                        tableDescription = it.AfterData?.FirstOrDefault()?.TableDescription;
                        afterData = it.AfterData.Select(r => r.Columns.Where(c => (c.Value??"").ToString().IsNotEmpty()).Select(c => new { c.ColumnName, c.ColumnDescription, c.Value })
                        ).ToJson();


                        break;
                    case DiffType.update:
                        tableName = it.AfterData?.FirstOrDefault()?.TableName;

                        tableDescription = it.AfterData?.FirstOrDefault()?.TableDescription;

                        beforeData = it.AfterData.Select(r => r.Columns.Select(c => new { c.ColumnName, c.ColumnDescription, c.Value })
                         ).ToJson();

                        afterData = it.AfterData.Select(r => r.Columns.Where(c => c.Value.ToString().IsNotEmpty()).Select(c => new { c.ColumnName, c.ColumnDescription, c.Value })
                        ).ToJson();

                        break;
                    case DiffType.delete:
                        tableName = it.BusinessData.ToString();

                        break;
                    default:
                        break;
                }

                //参数
                var parameter = it.Parameters.Where(r => r.Value.ToString().IsNotEmpty())
                .Select(r => new Dictionary<string, object> { { r.ParameterName, r.Value } }).ToJson();

                //插入审计表（这里无实体引用）
                var dc = new Dictionary<string, object>
                {
                    { "TableName", tableName },
                    { "DiffType", it.DiffType },
                    { "BeforeData", beforeData },
                    { "AfterData", afterData },
                    { "Sql", it.Sql },
                    { "Parameter",parameter },
                    { "TotalMilliseconds",  it.Time?.TotalMilliseconds },
                    { "BusinessData", it.BusinessData }, //业务参数
                    { "CreatedId", App.User?.FindFirstValue("UserId").ToInt() },
                    { "CreatedTime", DateTime.Now }
                };
                Db.Insertable(dc).AS("Sys_AuditLog").ExecuteCommand();
            };

            #endregion
        }
    }
}



