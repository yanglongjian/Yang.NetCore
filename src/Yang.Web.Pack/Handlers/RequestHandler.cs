using Furion;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Web.Pack
{
    /// <summary>
    /// 请求日志处理中间件
    /// </summary>
    public class RequestHandler : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            // 拦截之前
            var requestIp = actionContext.HttpContext.GetClientIp();
            var start = Stopwatch.GetTimestamp();
            ActionExecutedContext result = await next();        
            #region 拦截后执行 记录日志        

            string requestId = actionContext.HttpContext.TraceIdentifier;
            var requestPath = actionContext.HttpContext.Request.Path.ToString();

            var args = actionContext.ActionArguments.ToJson();
            var elapsed = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
         
            string message;
            AjaxResultType code;
            if (result.Exception.IsNotNull())
            {
                code = AjaxResultType.Error;
                message = result.Exception?.Message ?? "";
            }
            else
            {
                var ajaxResult= (AjaxResult)((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value;
                code = ajaxResult.Code;
                message = ajaxResult.Message;
            }

            //插入日志
            var context = App.GetService<DbContext>();
            await context.Db.Insertable(new RequestLog
            {
                Id = requestId,
                RequestPath = requestPath,
                RequestIp = requestIp,
                Args = args,
                Elapsed = elapsed,
                Message = message,
                StackTrace = result.Exception?.StackTrace ?? "",
                Code = code,
            }).ExecuteCommandAsync();


            #endregion 
        }

        /// <summary>
        /// 获取耗时
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }
    }
}



