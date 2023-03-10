using AspectCore.DynamicProxy;
using Furion.Logging.Extensions;
using Furion.UnifyResult;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Yang.Core
{
    /// <summary>
    /// AOP 事务
    /// </summary>
    public class UnitOfWorkAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var dbContext = context.ServiceProvider.GetService<DbContext>();
            try
            {
                dbContext.Db.BeginTran();
                var result = (AjaxResult)(await RunAndGetReturn(context, next));
                if (result.Code == AjaxResultType.Success)
                {
                    dbContext.Db.CommitTran();
                }
                else {
                    dbContext.Db.RollbackTran();
                }

            }
            catch (AggregateException ex)
            {
                ex.Message.LogError<UnitOfWorkAttribute>();
                dbContext.Db.RollbackTran();
            }
            catch (Exception ex)
            {
                (ex.Message + ex.StackTrace).LogError<UnitOfWorkAttribute>();
                dbContext.Db.RollbackTran();
                throw;
            }
        }



        /// <summary>
        /// 执行被拦截方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private static async Task<object> RunAndGetReturn(AspectContext context, AspectDelegate next)
        {
            await context.Invoke(next);
            return context.IsAsync()
            ? await context.UnwrapAsyncReturnValue()
            : context.ReturnValue;
        }
    }

}



