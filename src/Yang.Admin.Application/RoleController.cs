using Furion;
using Furion.DynamicApiController;
using Furion.LinqBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 角色信息
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("角色信息", "系统管理", Module = "Admin", OrderCode = 2)]
    public class RoleController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public RoleController(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页读取列表信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read(PageRequest request)
        {
            var predicate = FilterHelper.GetExpression<Role>(request.FilterRules);
            string sortConditions = FilterHelper.GetSortCondition<Role>(request.SortConditions);

            predicate = predicate.And(r => !r.IsSystem);

            RefAsync<int> total = 0;
            var rows = await _repository.Queryable<Role>().Where(predicate)
                 .OrderBy(sortConditions)
                 .ToPageListAsync(request.PageIndex, request.PageSize, total);

            return new AjaxResult(AjaxResultType.Success, new { rows, total = total.Value });
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(RoleInputDto dto)
        {
            var i = await _repository.InsertDto<Role, RoleInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success, i);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(RoleInputDto dto)
        {
            var i = await _repository.UpdateDto<Role, RoleInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success, i);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("删除")]
        public virtual async Task<AjaxResult> Delete([FromBody] int[] ids)
        {
            var i = await _repository.Delete<Role>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success, i);
        }

        /// <summary>
        /// 读取角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [ModuleInfo("读取角色权限")]
        public virtual async Task<AjaxResult> ReadRoleModules(int roleId)
        {
            var checkedKeys = await _repository.Queryable<ModuleRole>().Where(r => r.RoleId == roleId).Select(m => m.ModuleId).ToListAsync();
            var list = await _repository.Queryable<Module>().ToListAsync();
            var rows = ModulePermision.GetCheckedModules(list);
            return new AjaxResult(AjaxResultType.Success, new
            {
                checkedKeys,
                rows
            });
        }


        /// <summary>
        ///  设置角色权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [UnitOfWork]
        [ModuleInfo("设置角色权限")]
        public async Task<AjaxResult> SetRoleModules(ModuleRoleInputDto dto)
        {
            var role = await _repository.Queryable<Role>().FirstAsync(r => r.Id == dto.RoleId);
            if (role.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"编号为“{dto.RoleId}”的角色信息不存在");

            var moduleRoleList = await _repository.Queryable<ModuleRole>().Where(r => r.RoleId == dto.RoleId).ToListAsync();
            return await ModulePermision.SetRoleModules(role, moduleRoleList, dto.ModuleIds);
        }

    }


    /// <summary>
    /// 模块权限扩展
    /// </summary>
    public class ModulePermision
    {

        /// <summary>
        /// 获取模块勾选树
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<Module> GetCheckedModules(List<Module> modules, string parentId = "")
        {
            List<Module> nodes = new();
            foreach (var item in modules.Where(r => r.ParentId == parentId).OrderBy(r => r.OrderCode))
            {
                var children = GetCheckedModules(modules, item.Id);
                var node = new Module
                {
                    Id = item.Id,
                    Name = item.Name,
                    AccessType = item.AccessType,
                    Children = children
                };

                //有子节点并且设置角色访问
                if (node.Children.Count == 0 && node.AccessType != AccessType.RoleLimit)
                {
                    continue;
                }

                nodes.Add(node);
            }
            return nodes;

        }


        /// <summary>
        /// 设置用户权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userList"></param>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        public static async Task<AjaxResult> SetUserModules(User user, List<ModuleUser> userList, string[] selectedIds)
        {
            List<string> addNames = new(), removeNames = new();
            List<ModuleUser> addList = new();
            List<int> removeList = new();

            #region 权限插入删除判断

            //现有权限
            var existIds = userList.Select(r => r.ModuleId).ToArray();
            //添加权限
            var addIds = selectedIds.Except(existIds).ToArray();
            //删除权限
            var removeIds = existIds.Except(selectedIds).ToArray();

            var _cache = App.GetService<IMemoryCache>();
            var modules = _cache.Get<Module[]>(AdminConstants.SystemModuleKey);

            foreach (var id in addIds)
            {
                var module = modules.FirstOrDefault(r => r.Id == id);
                if (module.IsNull()) return new AjaxResult(AjaxResultType.Error, $"编号为“{id}”的模块信息不存在");

                addNames.Add(module.Name);
                addList.Add(new ModuleUser() { ModuleId = id, UserId = user.Id });

            }
            foreach (var id in removeIds)
            {
                var module = modules.FirstOrDefault(r => r.Id == id);
                if (module.IsNull()) return new AjaxResult(AjaxResultType.Error, $"编号为“{id}”的模块信息不存在");


                var entity = userList.FirstOrDefault(r => r.UserId == user.Id && r.ModuleId == id);
                if (entity.IsNull()) continue;

                removeNames.Add(module.Name);
                removeList.Add(entity.Id);
            }

            #endregion


            //更新缓存使设置生效

            _cache.Set($"{AdminConstants.UserModulePrefix}{user.Id}", existIds.Union(addIds).Except(removeIds).Distinct().ToArray());

            //操作记录
            int count = 0;
            var _repository = App.GetService<IRepository>();
            if (addList.Count > 0)
            {
                count += await _repository.InsertBatch(addList);
            }
            if (removeList.Count > 0)
            {
                count += await _repository.Delete<ModuleUser>(r => removeList.Contains(r.Id));
            }

            if (count == 0) return new AjaxResult(AjaxResultType.NoChanged, count);

            StringBuilder sb = new();
            if (addNames.Count > 0 && removeNames.Count == 0)
            {
                sb.Append($"添加模块“{addNames.ExpandAndToString()}”");
            }
            if (addNames.Count == 0 && removeNames.Count > 0)
            {
                sb.Append($"移除模块“{removeNames.ExpandAndToString()}”");
            }
            else
            {
                sb.Append($"添加模块“{addNames.ExpandAndToString()}”;移除模块“{removeNames.ExpandAndToString()}”");
            }
            return new AjaxResult(AjaxResultType.Success, sb.ToString());
        }


        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="role"></param>
        /// <param name="moduleRoleList"></param>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        public static async Task<AjaxResult> SetRoleModules(Role role, List<ModuleRole> moduleRoleList, string[] selectedIds)
        {
            List<string> addNames = new(), removeNames = new();
            List<ModuleRole> addList = new();
            List<int> removeList = new();

            #region 权限插入删除判断

            //现有权限
            var existIds = moduleRoleList.Select(r => r.ModuleId).ToArray();
            //添加权限
            var addIds = selectedIds.Except(existIds).ToArray();
            //删除权限
            var removeIds = existIds.Except(selectedIds).ToArray();

            var _cache = App.GetService<IMemoryCache>();
            var modules = _cache.Get<Module[]>(AdminConstants.SystemModuleKey);

            foreach (var id in addIds)
            {
                var module = modules.FirstOrDefault(r => r.Id == id);
                if (module.IsNull()) return new AjaxResult(AjaxResultType.Error, $"编号为“{id}”的模块信息不存在");

                addNames.Add(module.Name);
                addList.Add(new ModuleRole() { ModuleId = id, RoleId = role.Id });

            }
            foreach (var id in removeIds)
            {
                var module = modules.FirstOrDefault(r => r.Id == id);
                if (module.IsNull()) return new AjaxResult(AjaxResultType.Error, $"编号为“{id}”的模块信息不存在");


                var entity = moduleRoleList.FirstOrDefault(r => r.RoleId == role.Id && r.ModuleId == id);
                if (entity.IsNull()) continue;

                removeNames.Add(module.Name);
                removeList.Add(entity.Id);
            }

            #endregion


            //更新缓存使设置生效
            _cache.Set($"{AdminConstants.RoleModulePrefix}{role.Id}", existIds.Union(addIds).Except(removeIds).Distinct().ToArray());

            //操作记录
            int count = 0;
            var _repository = App.GetService<IRepository>();
            if (addList.Count > 0)
            {
                count += await _repository.InsertBatch(addList);
            }
            if (removeList.Count > 0)
            {
                count += await _repository.Delete<ModuleRole>(r => removeList.Contains(r.Id));
            }

            if (count == 0) return new AjaxResult(AjaxResultType.NoChanged, count);

            StringBuilder sb = new();
            if (addNames.Count > 0 && removeNames.Count == 0)
            {
                sb.Append($"添加模块“{addNames.ExpandAndToString()}”");
            }
            if (addNames.Count == 0 && removeNames.Count > 0)
            {
                sb.Append($"移除模块“{removeNames.ExpandAndToString()}”");
            }
            else
            {
                sb.Append($"添加模块“{addNames.ExpandAndToString()}”;移除模块“{removeNames.ExpandAndToString()}”");
            }
            return new AjaxResult(AjaxResultType.Success, sb.ToString());
        }

    }
}



