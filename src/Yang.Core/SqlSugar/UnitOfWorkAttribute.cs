using AspectCore.DynamicProxy;
using Furion.Logging.Extensions;
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
            try
            {
                DbContext.Instance.BeginTran();
                await RunAndGetReturn(context, next);
                DbContext.Instance.CommitTran();

            }
            catch (AggregateException ex)
            {
                ex.Message.LogError<UnitOfWorkAttribute>();
                DbContext.Instance.RollbackTran();
            }
            catch (Exception ex)
            {
                (ex.Message + ex.StackTrace).LogError<UnitOfWorkAttribute>();
                DbContext.Instance.RollbackTran();
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



