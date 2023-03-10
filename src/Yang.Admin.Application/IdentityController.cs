using Furion;
using Furion.DataEncryption;
using Furion.DynamicApiController;
using Furion.EventBus;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 登录认证
    /// </summary>
    [NonUnify]
    [LoggedIn]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("登录认证", "系统管理", Module = "Admin")]
    public class IdentityController : IDynamicApiController
    {
        private readonly IRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IEventPublisher _eventPublisher;
        /// <summary>
        /// 
        /// </summary>
        public IdentityController(IRepository repository, IMemoryCache cache, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _cache = cache;
            _eventPublisher = eventPublisher;
        }



        /// <summary>
        /// 获取身份认证Token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ModuleInfo("获取身份认证Token")]
        public virtual async Task<AjaxResult> Token(LoginInputDto dto)
        {

            var user = await _repository.Queryable<User>().Includes(u => u.UserRoles).FirstAsync(r => r.Account == dto.Account);
            if (user.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"账号 “{dto.Account}” 不存在");
            if (user.Status != 0) return new AjaxResult(AjaxResultType.Warning, $"账号 “{dto.Account}” 已被禁用,请联系管理员");
            if (user.Password != dto.Password) return new AjaxResult(AjaxResultType.Error, $"账号 “{dto.Account}” 密码不正确");


            var _accessor = App.GetRequiredService<IHttpContextAccessor>();
            dto.ClientIp = _accessor.HttpContext.GetClientIp();
            dto.UserAgent = _accessor.HttpContext.GetUserAgent();
            await _eventPublisher.PublishAsync(new ChannelEventSource("Create:LoginLog", new LoginLog
            {
                UserId = user.Id,
                ClientIp = dto.ClientIp,
                UserAgent = dto.UserAgent
            }));



            //设置角色/权限缓存
            var userPermissionCodes = await _repository.Queryable<ModuleUser>()
                 .LeftJoin<Module>((mu, m) => mu.ModuleId == m.Id)
                 .Where(mu => mu.UserId == user.Id)
                 .Select((mu, m) => m.Code).ToListAsync();

            _cache.Set($"{AdminConstants.UserRolePrefix}{user.Id}", user.UserRoles.Select(r => r.RoleId).ToArray());
            _cache.Set($"{AdminConstants.UserModulePrefix}{user.Id}", userPermissionCodes.ToArray());


            var tokenInfo = new TokenInfo
            {
                UserId = user.Id,
                Account = user.Account,
                Avatar = user.Avatar,
                NickName = user.NickName,
                IsSystem = user.IsSystem,
                Ip = dto.ClientIp,
                UserType = UserType.Admin,
                LoginTime = DateTime.Now
            };
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(tokenInfo));
            var accessToken = JWTEncryption.Encrypt(dict);
            // 获取刷新 token 刷新token有效期（分钟）设置7天
            var refreshTime = DateTime.Now.AddMinutes(10080).ToJsGetTime().ToLong();
            var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, 10080);
            return new AjaxResult(AjaxResultType.Success, new
            {
                accessToken,
                refreshToken,
                refreshTime
            });
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("用户登出")]
        public virtual async Task<AjaxResult> Logout()
        {
            var userId = ClaimsExtension.UserId;
            _cache.Remove($"{AdminConstants.UserRolePrefix}{userId}");
            _cache.Remove($"{AdminConstants.UserModulePrefix}{userId}");
            await _eventPublisher.PublishAsync(new ChannelEventSource("Update:LoginLog", userId));
            return new AjaxResult(AjaxResultType.Success, true);
        }



        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("获取用户信息")]
        public virtual AjaxResult GetUserInfo()
        {
            var _cache = App.GetService<IMemoryCache>();
            var modules = _cache.Get<Module[]>(AdminConstants.SystemModuleKey);
            var userInfo = ClaimsExtension.User;

            if (userInfo.IsSystem) return new AjaxResult(AjaxResultType.Success, new
            {
                userInfo,
                permissions = modules.Select(r => r.Code)
            });

            #region 获取用户权限
            var permissionCodes = new List<string>();
            //用户权限
            var userCodes = _cache.Get<string[]>($"{AdminConstants.UserModulePrefix}{userInfo.UserId}");
            if (userCodes.IsNotNull()) permissionCodes.AddRange(userCodes);

            //角色权限
            var roleIds = _cache.Get<int[]>($"{AdminConstants.UserRolePrefix}{userInfo.UserId}");
            if (roleIds.IsNotNull())
            {
                foreach (var roleId in roleIds)
                {
                    var roleCodes = _cache.Get<string[]>($"{AdminConstants.RoleModulePrefix}{roleId}");
                    if (roleCodes.IsNotNull()) permissionCodes.AddRange(roleCodes);
                }
            }
            #endregion

            //判断权限类型
            var result = new List<string>();
            foreach (var module in modules)
            {
                if (module.AccessType == AccessType.RoleLimit)
                {
                    if (permissionCodes.Contains(module.Code) || modules.Any(r => r.ParentId == module.Id && permissionCodes.Contains(r.Code)))
                    {
                        result.Add(module.Code);
                    }
                }
                else
                {
                    result.Add(module.Code);
                }
            }


            return new AjaxResult(AjaxResultType.Success, new
            {
                userInfo,
                permissions = result.Distinct()
            });
        }



        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("修改密码")]
        public virtual async Task<AjaxResult> ChangePassword(UserPasswordInputDto dto)
        {
            if (dto.ConfirmNewPassword != dto.NewPassword) throw Oops.Bah("新密码与确认密码不一致");

            dto.UserId = (App.User?.FindFirstValue("UserId")).ToInt();
            var user = await _repository.Queryable<User>().FirstAsync(r => r.Id == dto.UserId);
            if (user.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"用户编号“{dto.UserId}” 信息不存在");
            if (user.Password != dto.OldPassword) return new AjaxResult(AjaxResultType.NoFound, $"原密码不正确");

            user.Password = MD5Encryption.Encrypt(dto.NewPassword);
            int i = await _repository.Update(user, "Password");
            return new AjaxResult(AjaxResultType.Success, i); ;
        }
    }
}



