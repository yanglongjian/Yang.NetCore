using Furion.EventBus;
using Yang.Admin.Domain;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 登录认证
    /// </summary>
    [LoggedIn]
    [ModuleInfo("登录认证", "系统管理", Module = "Admin")]
    public class IdentityController : IDynamicApiController
    {
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 
        /// </summary>
        public IdentityController(IMemoryCache cache)
        {
            _cache = cache;
        }



        /// <summary>
        /// 获取身份认证Token
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ModuleInfo("获取身份认证Token")]
        public virtual async Task<object> Token(LoginInputDto dto)
        {
            var user = await DbContext.Instance.Queryable<User>()
                .Includes(r => r.Detail).Includes(u => u.UserRoles).FirstAsync(r => r.Account == dto.Account);

            if (user.IsNull()) throw Oops.Bah($"账号 “{dto.Account}” 不存在");
            if (user.Status == 0) throw Oops.Bah($"账号 “{dto.Account}” 已被禁用,请联系管理员");
            if (user.Detail.Password != dto.Password) throw Oops.Bah($"账号 “{dto.Account}” 密码不正确");

            //生成token
            var token = user.Adapt<TokenInfo>();
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(token));
            var accessToken = JWTEncryption.Encrypt(dict);
            var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, 10080);
            return new
            {
                accessToken,
                refreshToken
            };
        }



        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("获取用户信息")]
        public virtual async Task<User> GetUserInfo()
        {
            var userId = App.User?.FindFirstValue("UserId").ToInt();
            var user = await _cache.GetOrCreateAsync($"{Constants.UserInfoPrefix}{userId}", async item =>
            {
                var user = await DbContext.Instance.Queryable<User>()
                .Includes(u => u.UserRoles)
                .FirstAsync(r => r.Id == userId);

                var modules = _cache.Get<Module[]>(Constants.SystemModule);
                if (user.IsSystem)
                {
                    user.PermissionCodeList = modules.Select(r => r.Code).ToList();
                }
                else
                {
                    //获取所有模块编号
                    //var result = new List<string>();
                    //foreach (var module in modules)
                    //{
                    //    if (module.AccessType == AccessType.RoleLimit)
                    //    {
                    //        if (permissionCodes.Contains(module.Code) || modules.Any(r => r.ParentId == module.Id && permissionCodes.Contains(r.Code)))
                    //        {
                    //            result.Add(module.Code);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        result.Add(module.Code);
                    //    }
                    //}
                }
                return user;
            });
            return user;
        }


        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("用户登出")]
        public virtual bool Logout()
        {
            var userId = App.User?.FindFirstValue("UserId");
            _cache.Remove($"{Constants.UserInfoPrefix}{userId}");
            return true;
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [LoggedIn]
        [ModuleInfo("修改密码")]
        public virtual async Task<int> ChangePassword(SetPasswordInputDto dto)
        {
            if (dto.ConfirmNewPassword != dto.NewPassword) throw Oops.Bah("新密码与确认密码不一致");
            dto.UserId = (App.User?.FindFirstValue("UserId")).ToInt();
            var userDetail = await DbContext.Instance.Queryable<UserDetail>().FirstAsync(r => r.UserId == dto.UserId);
            if (userDetail.IsNull()) throw Oops.Bah($"用户编号“{dto.UserId}” 信息不存在");

            if (userDetail.Password != dto.OldPassword) throw Oops.Bah($"原密码不正确");
            userDetail.Password = MD5Encryption.Encrypt(dto.NewPassword);
            return await DbContext.Instance.Updateable(userDetail).UpdateColumns("Password").ExecuteCommandAsync();
        }
    }
}



