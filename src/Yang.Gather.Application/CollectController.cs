using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Core;
using Yang.Gather.Application.Dtos;
using Yang.Gather.Domain;

namespace Yang.Gather.Application
{
    /// <summary>
    /// 采集管理
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("采集管理", "站点管理", Module = "CMS", OrderCode = 6)]
    public class CollectController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public CollectController(IRepository repository)
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
            //var columnList = await _repository.Queryable<Column>().ToListAsync();

            var predicate = FilterHelper.GetExpression<Collect>(request.FilterRules);
            var orderFields = FilterHelper.GetSortCondition<Collect>(request.SortConditions);
            RefAsync<int> total = 0;
            var rows = await _repository.Queryable<Collect>()
                .Where(predicate)
                .OrderBy(orderFields)
                .ToPageListAsync(request.PageIndex, request.PageSize, total);

            //foreach (var item in rows)
            //{
            //    var column = columnList.FirstOrDefault(r => r.Id == item.ColumnId);
            //    item.ColumnName = column?.Name ?? "";
            //    if (item.ColumnName.IsNotEmpty())
            //        item.ColumnName = GetColumnName(column.ParentId, item.ColumnName, columnList);
            //}
            return new AjaxResult(AjaxResultType.Success, new
            {
                rows,
                total = total.Value
            });
        }

        //private string GetColumnName(int parentId, string name, List<Column> columnList)
        //{
        //    if (parentId == 0) return name;

        //    string result = string.Empty;
        //    var column = columnList.FirstOrDefault(r => r.Id == parentId);
        //    if (column.IsNotNull())
        //    {
        //        result = GetColumnName(column.ParentId, column.Name + "->" + name, columnList);
        //    }
        //    return result;
        //}






        /// <summary>
        /// 获取采集信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ModuleInfo("获取采集信息")]
        public virtual async Task<AjaxResult> GetById(int id)
        {
            var data = await _repository.Queryable<Collect>().Includes(r => r.ConfigList).FirstAsync(r => r.Id == id);
            return new AjaxResult(AjaxResultType.Success, data);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [UnitOfWork]
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(CollectInputDto dto)
        {
            dto.NextTime = System.DateTime.Now.AddMinutes(dto.Interval);
            var collectId = await _repository.InsertDto<Collect, CollectInputDto>(dto);

            foreach (var item in dto.ConfigList)
            {
                item.Index = dto.ConfigList.IndexOf(item) + 1;
                item.CollectId = collectId;
            }
            await _repository.InsertBatch(dto.ConfigList);
            return new AjaxResult(AjaxResultType.Success, 1);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [UnitOfWork]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(CollectInputDto dto)
        {
            dto.NextTime = System.DateTime.Now.AddMinutes(dto.Interval);
            await _repository.UpdateDto<Collect, CollectInputDto>(dto);

            await _repository.Delete<CollectConfig>(r => r.CollectId == dto.Id);
            foreach (var item in dto.ConfigList)
            {
                item.Index = dto.ConfigList.IndexOf(item) + 1;
                item.CollectId = dto.Id;
            }
            await _repository.InsertBatch(dto.ConfigList);
            return new AjaxResult(AjaxResultType.Success, 1);
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
            await _repository.Delete<Collect>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success);
        }


        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[HttpPost]
        //[ModuleInfo("执行")]
        //public virtual async Task<AjaxResult> Run(CollectDto dto)
        //{
        //    var collect = await _repository.Queryable<Collect>().Includes(r => r.ConfigList).FirstAsync(r => r.Id == dto.Id);
        //    if (collect.IsNull()) return new AjaxResult(AjaxResultType.NoFound, "采集任务不存在");
        //    Task.Run(() =>
        //    {
        //        CollectHelper.Run(_repository, collect);
        //    });
        //    return new AjaxResult(AjaxResultType.Success, "执行成功,数据采集中...");
        //}


        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("克隆")]
        public virtual async Task<AjaxResult> Clone(CollectDto dto)
        {
            var collect = await _repository.Queryable<Collect>().Includes(r => r.ConfigList).FirstAsync(r => r.Id == dto.Id);
            if (collect.IsNull()) return new AjaxResult(AjaxResultType.NoFound, "克隆的采集任务不存在");


            var newCollect = collect.ToJson().ToObject<Collect>();
            newCollect.Id = 0;
            newCollect.Name = $"克隆{DateTime.Now:yyyyMMdd}";
            newCollect.NextTime = System.DateTime.Now.AddMinutes(newCollect.Interval);

            var newId = await _repository.Insert(newCollect);
            foreach (var item in newCollect.ConfigList)
            {
                item.Id = 0;
                item.CollectId = newId;
            }
            await _repository.InsertBatch(newCollect.ConfigList);
            return new AjaxResult(AjaxResultType.Success, "克隆成功");
        }


        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[HttpPost]
        //[ModuleInfo("预览")]
        //public virtual async Task<AjaxResult> Preview(CollectInputDto dto)
        //{
        //    var collect = dto.Adapt<Collect>();
        //    var displaceList = await _repository.Queryable<Displace>().ToListAsync();
        //    (var total, var obj) = await CollectHelper.HandleGather(_repository, displaceList,collect,true);
        //    return new AjaxResult(AjaxResultType.Success, obj);
        //}

    }
}


