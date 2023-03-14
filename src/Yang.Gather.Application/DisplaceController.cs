using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Core;
using Yang.Gather.Application.Dtos;
using Yang.Gather.Domain;

namespace Yang.Gather.Application
{
    /// <summary>
    /// 替换管理
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("替换管理", "站点管理", Module = "CMS", OrderCode = 7)]
    public class DisplaceController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public DisplaceController(IRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        ///  读取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read(PageRequest request)
        {
            var predicate = FilterHelper.GetExpression<Displace>(request.FilterRules);
            var orderFields = FilterHelper.GetSortCondition<Displace>(request.SortConditions);
            RefAsync<int> total = 0;
            var rows = await _repository.Queryable<Displace>()
                .Where(predicate)
                .OrderBy(orderFields)
                .ToPageListAsync(request.PageIndex, request.PageSize, total);

            return new AjaxResult(AjaxResultType.Success, new
            {
                rows,
                total = total.Value
            });
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(BatchDisplaceInputDto dto)
        {
            var keys = dto.Content.Split("\n");
            var list = new List<Displace>();
            foreach (var key in keys)
            {
                if (key.IsNotEmpty())
                {
                    var words = key.Split("|");

                    list.Add(new Displace
                    {
                        Word = words[0],
                        NewWord = words[1]
                    });
                }
            }
            var i = await _repository.InsertBatch(list);
            return new AjaxResult(AjaxResultType.Success, i);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(DisplaceInputDto dto)
        {
            await _repository.UpdateDto<Displace, DisplaceInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success,1);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("删除")]
        public virtual async Task<AjaxResult> Delete([FromBody] int[] ids)
        {
            await _repository.Delete<Displace>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success);
        }
    }
}


