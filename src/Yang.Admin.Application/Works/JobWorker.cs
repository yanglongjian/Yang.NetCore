using Furion;
using Furion.Logging.Extensions;
using Furion.TaskScheduler;
using System;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Admin.Application.Works
{
    /// <summary>
    /// 定时任务(不支持构造函数注入依赖项)
    /// </summary>
    public class JobWorker : ISpareTimeWorker
    {

        /// <summary>
        /// 删除日志(每天午夜运行一次)
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [SpareTime("0 1 0 * * ? ", "DeleteLog", StartNow = true, ExecuteType = SpareTimeExecuteTypes.Serial)]
        public virtual async Task DeleteLog(SpareTimer timer, long count)
        {
            var _repository = App.GetService<IRepository>();
            var endTime = DateTime.Now.Date.AddDays(-6);
            var i = await _repository.Delete<LoginLog>(r => r.CreatedTime < endTime);
            var j = await _repository.Delete<RequestLog>(r => r.CreatedTime < endTime);
            $"[{timer.WorkerName}]:({count}) 删除登录日志{i},请求日志{j}".LogInformation();
        }
    }
}







