//using Furion;
//using Furion.Logging.Extensions;
//using Furion.TaskScheduler;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Yang.Core;
//using Yang.Gather.Domain;

//namespace Yang.Gather.Application.Works
//{
//    /// <summary>
//    /// 定时任务(不支持构造函数注入依赖项)
//    /// </summary>
//    public class JobWorker : ISpareTimeWorker
//    {
//        /// <summary>
//        /// 采集任务
//        /// </summary>
//        /// <param name="timer"></param>
//        /// <param name="count"></param>
//        /// <returns></returns>
//        [SpareTime(5000, "GatherHandle", StartNow = true, ExecuteType = SpareTimeExecuteTypes.Serial)]
//        public virtual async Task GatherHandle(SpareTimer timer, long count)
//        {
//            var _repository = App.GetService<IRepository>();
//            var collectList = await _repository.Queryable<Collect>()
//                .Includes(r => r.ConfigList)
//                .Where(r => r.Status == 0 && r.Interval > 0 && r.NextTime < DateTime.Now)
//                .ToListAsync();

//            foreach (var item in collectList)
//            {
//                await CollectHelper.Run(_repository, item, true);
//            }
//        }
//    }
//}






