using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 请求日志
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("请求日志", "系统管理", Module = "Admin", OrderCode = 9)]
    public class RequestLogController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public RequestLogController(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 分页读取列表信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read(PageRequest request)
        {
            var predicate = FilterHelper.GetExpression<RequestLogOutputDto>(request.FilterRules);
            var sortConditions = FilterHelper.GetSortCondition<RequestLogOutputDto>(request.SortConditions, "CreatedTime", OrderByType.Desc);

            RefAsync<int> total = 0;
            var rows = await _repository.Queryable<RequestLog>()
                .LeftJoin<User>((r, u) => r.CreatedId == u.Id)
                .Select((r, u) => new RequestLogOutputDto { Id = r.Id.SelectAll(), Account = u.Account })
                .MergeTable()//将查询结果集变成表MergeTable
                .Where(predicate)
                .OrderBy(sortConditions)
                .ToPageListAsync(request.PageIndex, request.PageSize, total);


            return new AjaxResult(AjaxResultType.Success, new { rows, total = total.Value });
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ModuleInfo("获取详情")]
        public virtual async Task<AjaxResult> GetById(string id)
        {
            var log = await _repository.Queryable<RequestLog>().FirstAsync(r => r.Id == id);
            if (log.IsNull()) return new AjaxResult(AjaxResultType.NoFound, $"编号为“{id}”的日志信息不存在");
            return new AjaxResult(AjaxResultType.Success, log);

        }
    }
}



