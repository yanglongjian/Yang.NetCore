using Furion;
using Furion.Logging.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Web.Pack
{
    /// <summary>
    /// 功能模块初始化
    /// </summary>
    public class ModulePack
    {

        /// <summary>
        /// 初始化功能模块
        /// </summary>
        /// <param name="dllName"></param>
        public static void InitModule()
        {
            var context = App.GetService<DbContext>();

            var moduleList = GetControllers();

            #region 根据模块postion划分

            var postionList = moduleList.Where(r => !string.IsNullOrWhiteSpace(r.Postion)).GroupBy(r => r.Postion);
            foreach (var item in postionList)
            {
                var topPostion = new Admin.Domain.Module
                {
                    Id = Id.NextId(),
                    ParentId = "",
                    Postion = item.Key,
                    Area = item.FirstOrDefault().Area,
                    Name = item.Key,
                    Code = "Root." + item.FirstOrDefault().Area,
                    Controller = "",
                    Action = "",
                    IsController = false,
                    AccessType = AccessType.Anonymous
                };
                moduleList.Add(topPostion);
                foreach (var child in item)
                {
                    child.ParentId = topPostion.Id;
                }
            }

            #endregion

            #region 模块信息添加更新到数据库
            //获取数据库数据
            context.Db.BeginTran();
            var modules = context.Db.Queryable<Admin.Domain.Module>().ToArray();
            //删除模块
            var deleteCodes = modules.Select(r => r.Code).Except(moduleList.Select(r => r.Code)).ToArray();
            var deleteModules = modules.Where(r => deleteCodes.Contains(r.Code)).Select(r => r.Id).ToArray();


            foreach (var item in deleteModules)
            {
                context.Db.Deleteable<Admin.Domain.Module>().In(item).ExecuteCommand();

            }

            //新增或更新模块
            int addCount = 0;
            int updateCount = 0;
            foreach (var item in moduleList)
            {
                var module = modules.FirstOrDefault(r => r.Code == item.Code);
                if (module.IsNull())
                {
                    if (item.ParentId.IsNotEmpty())
                    {
                        //判断父节点是否存在赋值父节点Id
                        var parentCode = moduleList.FirstOrDefault(p => p.Id == item.ParentId)?.Code ?? "";
                        //数据库获取父节点代码
                        var parentModule = modules.FirstOrDefault(r => r.Code == parentCode);
                        if (parentModule.IsNotNull()) item.ParentId = parentModule.Id;
                    }
                    context.Db.Insertable(item).ExecuteCommand();
                    addCount++;
                }
                else
                {
                    var isUpdate = false;
                    if (!string.Equals(item.Name, module.Name, StringComparison.OrdinalIgnoreCase)) isUpdate = true;
                    if (item.Postion != module.Postion) isUpdate = true;
                    if (item.Area != module.Area) isUpdate = true;
                    if (item.OrderCode != module.OrderCode) isUpdate = true;
                    if (item.AccessType != module.AccessType) isUpdate = true;
                    if (isUpdate)
                    {
                        //以下字段不允许修改使用数据库源数据
                        item.Id = module.Id;
                        item.ParentId = module.ParentId;

                        context.Db.Updateable(item).ExecuteCommand();
                        updateCount++;
                    }
                }

            }
            #endregion

            $"[添加功能{addCount};更新功能{updateCount};删除功能{deleteCodes.Length}".LogInformation();
            context.Db.CommitTran();

            #region  添加到缓存中
            var saveModules = context.Db.Queryable<Admin.Domain.Module>().ToArray();
            var _cache = App.GetService<IMemoryCache>();
            _cache.Set(AdminConstants.SystemModuleKey, saveModules);

            //设置角色缓存
            var roles = context.Db.Queryable<Role>().Includes(u => u.Modules).ToList();
            foreach (var role in roles)
            {
                string key = $"{AdminConstants.RoleModulePrefix}{role.Id}";
                var moduleCodes = role.Modules.Select(r => r.Code).ToArray();
                if (moduleCodes.Length > 0)
                {
                    _cache.Set(key, moduleCodes);
                }
            }
            $"设置模块缓存 [{saveModules.Length}]".LogInformation();
            #endregion
        }


        /// <summary>
        /// 获取所有控制器及功能
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dll"></param>
        /// <returns></returns>
        private static List<Admin.Domain.Module> GetControllers()
        {
            var moduleList = new List<Admin.Domain.Module>();

            //获取控制器
            var controllers = AssemblyManager.FindTypesByAttribute<NonUnifyAttribute>();
            foreach (var controller in controllers)
            {
                AccessType accessType = GetAccessType(controller);
                if (controller.GetCustomAttributes(typeof(ModuleInfoAttribute), true).FirstOrDefault() is not ModuleInfoAttribute moduleInfo) 
                    throw new ArgumentException($"{controller.Name} 未配置moduleInfo信息");

                var area = moduleInfo?.Module ?? "";
                if (string.IsNullOrEmpty(area)) throw new ArgumentException($"{controller.Name} 未配置模块代码");

                var module = new Admin.Domain.Module
                {
                    Id=Id.NextId(),
                    ParentId = "",
                    Postion = moduleInfo.Position,
                    Area = area,
                    OrderCode = moduleInfo.OrderCode,
                    Name = moduleInfo.Name,
                    Code = "Root." + (area.IsEmpty() ? "" : area + ".") + controller.Name.Replace("Controller", ""),
                    Controller = controller.Name.Replace("Controller", ""),
                    Action = "",
                    IsController = true,
                    AccessType = accessType
                };
                moduleList.Add(module);
                GetActions(controller, module, moduleList);
            }
            return moduleList;
        }


        /// <summary>
        /// 获取控制器下的Action功能
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="module"></param>
        /// <param name="moduleList"></param>
        private static void GetActions(Type controller, Admin.Domain.Module module, List<Admin.Domain.Module> moduleList)
        {
            MethodInfo[] actions = controller.GetMethods((BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            int i = 1;
            foreach (var action in actions)
            {
                AccessType accessType = action.IsDefined(typeof(AllowAnonymousAttribute), true) ?
                    AccessType.Anonymous : (action.IsDefined(typeof(LoggedInAttribute), true) ? AccessType.LoggedIn : AccessType.RoleLimit);


                if (action.GetCustomAttributes(typeof(ModuleInfoAttribute), true).FirstOrDefault() is not ModuleInfoAttribute moduleInfo) 
                    throw new ArgumentException($"{action.Name} 未配置moduleInfo名称");

                moduleList.Add(new Admin.Domain.Module
                {
                    Id = Id.NextId(),
                    ParentId = module.Id,
                    Postion = moduleInfo.Position,
                    Area = module.Area,
                    OrderCode = i,
                    Name = moduleInfo.Name,
                    Code = "Root." + (module.Area.IsEmpty() ? "" : module.Area + ".") + $"{controller.Name.Replace("Controller", "")}.{action.Name}",
                    Controller = controller.Name.Replace("Controller", ""),
                    Action = action.Name,
                    IsController = false,
                    AccessType = accessType,
                });
                i++;
            }
        }


        /// <summary>
        /// 获取访问类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static AccessType GetAccessType(Type type)
        {
            var accessType =
            type.IsDefined(typeof(AllowAnonymousAttribute), true) ?
                AccessType.Anonymous :
                (type.IsDefined(typeof(LoggedInAttribute), true) ? AccessType.LoggedIn : AccessType.RoleLimit);
            return accessType;
        }

    }
}



