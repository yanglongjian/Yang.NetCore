using Furion;
using Furion.DataEncryption;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Furion.LinqBuilder;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("用户信息", "系统管理", Module = "Admin", OrderCode = 1)]
    public class UserController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public UserController(IRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        ///  分页读取列表信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read(int deptId, PageRequest request)
        {
            var predicate = FilterHelper.GetExpression<User>(request.FilterRules);
            var sortConditions = FilterHelper.GetSortCondition<User>(request.SortConditions);
            RefAsync<int> total = 0;

            predicate = predicate.And(r => r.IsSystem == false);
            if (deptId > 0)
            {
                predicate = predicate.And(r => r.DeptId == deptId);
            }

            //导航查询
            var list = await _repository.Queryable<User>().Includes(u => u.Roles.Where(r => r.Status == 0).ToList())
                .Includes(u => u.Dept).Includes(u => u.Post)
                 .Where(predicate)
                 .OrderBy(sortConditions)
                  .ToPageListAsync(request.PageIndex, request.PageSize, total);

            //Mapster 对象映射
            var _mapper = App.GetService<IMapper>();
            var rows = _mapper.Map<List<UserOutputDto>>(list);
            return new AjaxResult(AjaxResultType.Success, new { rows, total = total.Value });
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [UnitOfWork]
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(UserInputDto dto)
        {
            var exist = await _repository.IsExist<User>(r => r.Account == dto.Account);
            if (exist) return new AjaxResult(AjaxResultType.Warning, $"已存在 {dto.Account} 账号名称");

            var userId = await _repository.InsertDto<User, UserInputDto>(dto);

            var userRoleList = await _repository.Queryable<Role>().Where(r => r.IsDefault).Select(r => new UserRole { RoleId = r.Id, UserId = userId }).ToListAsync();
            await _repository.Insert(new UserDetail { UserId = userId });
            await _repository.InsertBatch(userRoleList);

            return new AjaxResult(AjaxResultType.Success, 1);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(UserInputDto dto)
        {
            var i = await _repository.UpdateDto<User, UserInputDto>(dto);
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
            var i = await _repository.Delete<User>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success, i);
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<User> GetUserById(int userId)
        {
            return await _repository.Queryable<User>().FirstAsync(r => r.Id == userId);
        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ModuleInfo("重置密码")]
        public virtual async Task<AjaxResult> ResetPassword(UserPasswordInputDto dto)
        {
            if (dto.ConfirmNewPassword != dto.NewPassword) throw Oops.Bah("新密码与确认密码不一致");

            var user = await GetUserById(dto.UserId);
            if (user.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"用户编号“{dto.UserId}” 信息不存在");

            user.Password = MD5Encryption.Encrypt(dto.NewPassword);
            int i = await _repository.Update(user, "Password");
            return new AjaxResult(AjaxResultType.Success, i);
        }


        /// <summary>
        /// 读取用户权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ModuleInfo("读取用户权限")]
        public virtual async Task<AjaxResult> ReadUserModules(int userId)
        {
            var checkedKeys = await _repository.Queryable<ModuleUser>().Where(r => r.UserId == userId).Select(m => m.ModuleId).ToListAsync();        
            var list = await _repository.Queryable<Module>().ToListAsync();
            var rows = ModulePermision.GetCheckedModules(list);
            return new AjaxResult(AjaxResultType.Success, new {
                checkedKeys,
                rows
            });
        }



        /// <summary>
        /// 设置用户权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [UnitOfWork]
        [ModuleInfo("设置用户权限")]
        public virtual async Task<AjaxResult> SetUserModules(ModuleUserInputDto dto)
        {
            var user = await GetUserById(dto.UserId);
            if (user.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"编号为“{dto.UserId}”的用户信息不存在");

            var moduleUserList = await _repository.Queryable<ModuleUser>().Where(r => r.UserId == dto.UserId).ToListAsync();
            return await ModulePermision.SetUserModules(user, moduleUserList, dto.ModuleIds);
        }


        /// <summary>
        /// 读取用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ModuleInfo("读取用户角色")]
        public virtual async Task<AjaxResult> ReadUserRoles(int userId)
        {
            var checkedKeys = await _repository.Queryable<UserRole>().Where(m => m.UserId == userId).Select(m => m.RoleId).ToListAsync();
            var rows = await _repository.Queryable<Role>().Where(m => m.Status == 0)
                .OrderBy(m => m.Id).OrderBy(m => m.IsAdmin, OrderByType.Desc)
                 .Select(r => new
                 {
                     Key = r.Id,
                     Title = r.Name
                 }).ToListAsync();
            return new AjaxResult(AjaxResultType.Success, new
            {
                checkedKeys,
                rows
            });
        }



        /// <summary>
        /// 设置角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [UnitOfWork]
        [ModuleInfo("设置角色")]
        public async Task<AjaxResult> SetRoles(UserSetRoleInputDto dto)
        {
            var user = await _repository.Queryable<User>().Includes(u => u.UserRoles).FirstAsync(r => r.Id == dto.UserId);
            if (user.IsNull()) throw Oops.Bah($"编号为“{dto.UserId}”的用户不存在");

            var existIds = user.UserRoles.Select(r => r.RoleId).ToArray();
            var addIds = dto.RoleIds.Except(existIds).ToArray();
            var removeIds = existIds.Except(dto.RoleIds).ToArray();


            if (!addIds.Union(removeIds).Any()) return new AjaxResult(AjaxResultType.NoChanged, "数据未发生变化");

            List<UserRole> addList = new();
            List<int> removeList = new();
            List<string> addNames = new(), removeNames = new();

            var roleList = await _repository.Queryable<Role>().Where(r => addIds.Union(removeIds).Contains(r.Id)).ToListAsync();
            foreach (var roleId in addIds)
            {
                var role = roleList.FirstOrDefault(r => r.Id == roleId);
                if (role.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"编号为“{roleId}”的角色信息不存在");

                addList.Add(new UserRole() { RoleId = roleId, UserId = dto.UserId });
                addNames.Add(role.Name);
            }
            foreach (var roleId in removeIds)
            {
                var role = roleList.FirstOrDefault(r => r.Id == roleId);
                if (role.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"编号为“{roleId}”的角色信息不存在");

                var userRole = user.UserRoles.FirstOrDefault(r => r.RoleId == roleId);
                if (userRole.IsNull()) continue;

                removeList.Add(userRole.Id);
                removeNames.Add(role.Name);
            }

            if (addList.Count > 0)
            {
                await _repository.InsertBatch(addList);
            }
            if (removeList.Count > 0)
            {
                await _repository.Delete<UserRole>(r => removeList.Contains(r.Id));
            }



            //更新缓存使设置生效
            var _cache = App.GetService<IMemoryCache>();
            var roleIds = existIds.Union(addIds).Except(removeIds).Distinct().ToArray();
            _cache.Set($"{AdminConstants.UserRolePrefix}{dto.UserId}", roleIds);


            if (addNames.Count > 0 && removeNames.Count == 0)
            {
                return new AjaxResult(AjaxResultType.Success, $"用户“{user.Account}”添加角色“{addNames.ExpandAndToString()}”操作成功");
            }
            if (addNames.Count == 0 && removeNames.Count > 0)
            {
                return new AjaxResult(AjaxResultType.Success, $"用户“{user.Account}”移除角色“{removeNames.ExpandAndToString()}”操作成功");
            }
            else
            {
                return new AjaxResult(AjaxResultType.Success, $"用户“{user.Account}”添加角色“{addNames.ExpandAndToString()}”，移除角色“{removeNames.ExpandAndToString()}”操作成功");
            }
        }
    }
}



