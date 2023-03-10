using Furion;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry
{
    /// <summary>
    /// 重定向中间件
    /// </summary>
    public class HostRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        public HostRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var host = context.Request.Headers.ToArray().FirstOrDefault(r => r.Key == "Host").Value.ToString();
            var ignoreOrigins = App.GetConfig<string[]>("IgnoreOrigins");
            if (!ignoreOrigins.Contains(host))
            {
                var path = context.Request.Path.ToString();
                //System.Console.WriteLine($"重定向：{host}--{path}");
                if (path.Contains("index.html") || path == "/")
                {
                    context.Response.Redirect($"/site");
                    return;
                }
            }
            await _next.Invoke(context);
            //404跳转
            if (!context.Response.HasStarted)
            {
                var response = context.Response;
                //如果是404就跳转到主页
                if (response.StatusCode == 404)
                {
                    response.Redirect("/notfound");
                }
            }
        }
    }
}

