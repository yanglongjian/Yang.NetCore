using Furion.DependencyInjection;
using Furion.EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Admin.Application.Events
{
    /// <summary>
    /// 登录日志事件
    /// </summary>
    public class LoginLogEventSubscriber : IEventSubscriber, ISingleton //只需要实现 ISingleton无需注册
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider Services { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public LoginLogEventSubscriber(IServiceProvider services)
        {
            Services = services;
        }


        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [EventSubscribe("Create:LoginLog")]
        public async Task CreateLoginLog(EventHandlerExecutingContext context)
        {
            using var scope = Services.CreateScope();
            var _repository = scope.ServiceProvider.GetRequiredService<IRepository>();
            var log = (LoginLog)context.Source.Payload;
            await _repository.Insert(log);
        }

        /// <summary>
        /// 登出事件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [EventSubscribe("Update:LoginLog")]
        public async Task UpdateLoginLog(EventHandlerExecutingContext context)
        {
            using var scope = Services.CreateScope();
            var _repository = scope.ServiceProvider.GetRequiredService<IRepository>();
            var userId = context.Source.Payload.ToInt();
            var log = await _repository.Queryable<LoginLog>().OrderByDescending(r => r.CreatedTime).FirstAsync(r => r.UserId == userId);
            log.LogoutTime = DateTime.Now;
            await _repository.Update(log,"LogoutTime");
        }

    }
}


