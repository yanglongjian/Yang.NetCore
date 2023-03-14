using CSRedis;
using Furion;
using Furion.DataEncryption;
using Furion.Logging.Extensions;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Web.Pack
{
    /// <summary>
    /// 数据种子
    /// </summary>
    public class DbSeedPack
    {
        /// <summary>
        /// 初始化数据表
        /// </summary>
        public static void InitTables()
        {
            //初始化redis
            //if (App.Configuration["ConnectionConfig:IsUseRedis"].ToBool())
            //{
            //    RedisHelper.Initialization(new CSRedisClient(App.Configuration["ConnectionConfig:Redis"]));
            //    "Redis初始化".LogInformation();
            //}

            //如果不存在创建数据库
            DbContext.Instance.DbMaintenance.CreateDatabase();

            //获取属性SugarTable的实体
            Type[] types = AssemblyManager.FindTypesByAttribute<SugarTable>();
            foreach (var type in types)
            {
                try
                {
                    //判断是否改变,改变才迁移
                    var diffString = DbContext.Instance.CodeFirst.GetDifferenceTables(type).ToDiffString();
                    if (!diffString.Contains("No change"))
                    {
                        $"[{type.Name}] 数据迁移".LogInformation();
                        DbContext.Instance.CodeFirst.InitTables(type);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex.StackTrace);
                }
            }
        }
    }
}


