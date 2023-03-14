using Furion;
using Furion.Logging.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;

namespace Yang.Core
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
           new ConnectionConfig {
               ConnectionString = App.Configuration["ConnectionConfigs:0:ConnectionString"],
               DbType = (DbType)App.Configuration["ConnectionConfigs:0:DbType"].ToInt(),
               IsAutoCloseConnection = App.Configuration["ConnectionConfigs:0:IsAutoCloseConnection"].ToBool(),
               InitKeyType = InitKeyType.Attribute,//从特性读取主键自增信息
               LanguageType = LanguageType.Chinese,
               ConfigureExternalServices = new ConfigureExternalServices()
               {
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
           }
        , db =>
        {
            //数据审计
            db.Aop.DataExecuting = (oldValue, entity) =>
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

            //Sql执行异常异常
            db.Aop.OnError = (exp) =>
            {
                $"sql执行异常\r\n:{exp.Sql}".LogError();
            };
            //数据审计日志
            db.Aop.OnDiffLogEvent = it =>
            {

            };
        });
    }
}



