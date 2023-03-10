using Furion.DynamicApiController;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Admin.Application
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("数据字典", "系统管理", Module = "Admin", OrderCode = 5)]
    public class DictController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public DictController(IRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read()
        {
            var rows = (await _repository.Queryable<Dict>().OrderBy(r => r.OrderCode).ToTreeAsync(it => it.Children, it => it.ParentId, 0)) ?? new List<Dict>();

            rows.ForEach(item =>
            {
                item.Children = item.Children.OrderBy(r => r.OrderCode).ToList();
            });
            return new AjaxResult(AjaxResultType.Success, rows);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(DictInputDto dto)
        {
            await _repository.InsertDto<Dict, DictInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success, 1);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(DictInputDto dto)
        {
            var i = await _repository.UpdateDto<Dict, DictInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success, i);
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
            var i = await _repository.Delete<Dict>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success, i);
        }


    }
}


