using Furion;
using Furion.DataEncryption;
using Furion.Logging.Extensions;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
            var context = App.GetService<DbContext>();
            //如果不存在创建数据库
            context.Db.DbMaintenance.CreateDatabase();

            //获取属性SugarTable的实体
            Type[] types = AssemblyManager.FindTypesByAttribute<SugarTable>();
            foreach (var type in types)
            {
                try
                {
                    //判断是否改变,改变才迁移
                    var diffString = context.Db.CodeFirst.GetDifferenceTables(type).ToDiffString();
                    if (!diffString.Contains("No change"))
                    {
                        $"[{type.Name}] 数据迁移".LogInformation();
                        context.Db.CodeFirst.InitTables(type);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex.StackTrace);
                }
            }
            SeedAuditEntity(context, types);
            UserSeedData(context);
        }


        /// <summary>
        ///  数据审计实体功能同步
        /// </summary>
        /// <param name="context"></param>
        /// <param name="models"></param>
        private static void SeedAuditEntity(DbContext context, Type[] types)
        {
            var entityList = context.Db.Queryable<Admin.Domain.EntityInfo>().ToList();

            //取差集删除
            var deleteEntityTypes = entityList.Select(r => r.TypeName).Except(types.Select(t => t.FullName)).ToArray();
            if (deleteEntityTypes.Length > 0)
            {
                context.Db.Deleteable<Admin.Domain.EntityInfo>().Where(r => deleteEntityTypes.Contains(r.TypeName)).ExecuteCommand();
            }
            List<Admin.Domain.EntityInfo> addlist = new();
            foreach (var t in types)
            {
                var tableInfo = t.GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault() as SugarTable;
                if (!entityList.Any(r => r.TypeName == t.FullName))
                {
                    addlist.Add(new Admin.Domain.EntityInfo
                    {
                        Name = tableInfo.TableDescription,
                        TableName = tableInfo.TableName,
                        TypeName = t.FullName,
                        IsAudit = false
                    });
                }
            }
            if (addlist.Count > 0)
            {
                context.Db.Insertable(addlist).ExecuteCommand();
            }
            $"实体初始化完成".LogInformation();
        }

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <param name="context"></param>
        private static void UserSeedData(DbContext context)
        {
            int roleId = 0;
            if (context.Db.Queryable<Role>().First().IsNull())
            {
                var role = new Role
                {
                    Name = "系统管理员",
                    IsSystem = true,
                    Remark = "系统最高权限管理角色",
                };
                roleId = context.Db.Insertable(role).ExecuteReturnIdentity();
            }
            if (context.Db.Queryable<User>().First().IsNull())
            {
                var user = new User
                {
                    Account = "admin@qq.com",
                    NickName = "管理员",
                    Password = MD5Encryption.Encrypt("just123123"),
                    Avatar = "https://avatars.githubusercontent.com/u/32290372",
                    Mobile = "18150370180",
                    Email = "admin@qq.com",
                    IsSystem = true,
                    UserDetail = new UserDetail(),
                    UserRoles = new List<UserRole>
                    {
                        new UserRole{  RoleId=roleId }
                    }
                };
                context.Db.Insertable(user)
                    .AddSubList(it => it.UserDetail.UserId)
                    .AddSubList(it => it.UserRoles.First().UserId)
                    .ExecuteCommand();
            }
            if (context.Db.Queryable<Setting>().First().IsNull())
            {
                context.Db.Insertable(new Setting
                {
                    SettingInfo = (new SettingInfo()).ToJson()
                }).ExecuteReturnIdentity();
            }
        }
    }
}


