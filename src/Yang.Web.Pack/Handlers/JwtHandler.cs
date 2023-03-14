using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
            var modules = _cache.Get<Module[]>(Constants.SystemModule);
            var code = httpContext.Request.Path.Value[1..].ToUpper().Replace("API", "ROOT").Replace("/", ".");
            var module = modules.FirstOrDefault(r => r.Code.ToUpper() == code);
            if (module.IsNull()) return Task.FromResult(true);
            if (module.AccessType != AccessType.RoleLimit) return Task.FromResult(true);

            var userId = App.User?.FindFirstValue("UserId");
            var userInfo = _cache.Get<User>($"{Constants.UserInfoPrefix}{userId}");
            // 管理员跳过判断
            if (userInfo.IsSystem) return Task.FromResult(true);
            if (userInfo.PermissionCodeList.Any(r => r.ToUpper() == code)) return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}


