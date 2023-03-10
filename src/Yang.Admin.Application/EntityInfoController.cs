using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 实体信息
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("实体信息", "系统管理", Module = "Admin", OrderCode = 7)]
    public class EntityInfoController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public EntityInfoController(IRepository repository)
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
            var (rows, total) = await _repository.GetPageList<EntityInfo>(request, "TypeName");
            return new AjaxResult(AjaxResultType.Success, new { rows, total });
        }

        /// <summary>
        /// 更新数据审计
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [UnitOfWork]
        [ModuleInfo("更新数据审计")]
        public virtual async Task<AjaxResult> Update(EntityAuditInputDto dto)
        {
            var columns = new List<Expression<Func<EntityInfo, bool>>>() {
                r => r.IsAudit == dto.IsAudit
            };
            var i = await _repository.Update(columns, r => dto.Ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success, i);
        }
    }
}



