using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 审计日志
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("审计日志", "系统管理", Module = "Admin", OrderCode =8)]
    public class AuditLogController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public AuditLogController(IRepository repository)
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
            var (rows, total) = await _repository.GetPageList<AuditLog>(request);
            return new AjaxResult(AjaxResultType.Success, new { rows, total });
        }


    }
}



