using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Web.Pack
{
    public class JwtHandler : AppAuthorizeHandler
    {
        /// <summary>
        /// 重写 Handler 添加自动刷新
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context)
        {
            // 自动刷新Token
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
            {
                await AuthorizeHandleAsync(context);
            }
            else context.Fail(); // 授权失败
        }

        /// <summary>
        /// 授权判断逻辑，授权通过返回 true，否则返回 false
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            // 此处已经自动验证 Jwt Token的有效性了，无需手动验证
            return await CheckAuthorzieAsync(httpContext);
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static Task<bool> CheckAuthorzieAsync(DefaultHttpContext httpContext)
        {
            var _cache = App.GetService<IMemoryCache>();
            var modules = _cache.Get<Module[]>(AdminConstants.SystemModuleKey);
            var routeCode = httpContext.Request.Path.Value[1..].ToUpper().Replace("API", "ROOT").Replace("/", ".");


            var module = modules.FirstOrDefault(r => r.Code.ToUpper() == routeCode);
            if (module.IsNull()) return Task.FromResult(true);
            if (module.AccessType != AccessType.RoleLimit) return Task.FromResult(true);


            // 管理员跳过判断
            var IsSystem = (App.User?.FindFirstValue("IsSystem")).ToBool();
            if (IsSystem) return Task.FromResult(true);

            var moduleCodes = GetModuleCodes(_cache);
            if (!moduleCodes.Distinct().Any(r => r.ToUpper() == routeCode))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }


        /// <summary>
        /// 获取用户&角色权限代码
        /// </summary>
        /// <param name="_cache"></param>
        /// <returns></returns>
        private static List<string> GetModuleCodes(IMemoryCache _cache)
        {
            var userId = App.User?.FindFirstValue("UserId");
            var roleIds = _cache.Get<int[]>($"{AdminConstants.UserRolePrefix}{userId}");
            var permissions = new List<string>();

            //权限判断
            var userCodes = _cache.Get<string[]>($"{AdminConstants.UserModulePrefix}{userId}");
            if (userCodes.IsNotNull()) permissions.AddRange(userCodes);

            foreach (var roleId in roleIds)
            {
                var roleCodes = _cache.Get<string[]>($"{AdminConstants.RoleModulePrefix}{roleId}");
                if (roleCodes.IsNotNull()) permissions.AddRange(roleCodes);
            }
            return permissions.Distinct().ToList();
        }


    }
}


