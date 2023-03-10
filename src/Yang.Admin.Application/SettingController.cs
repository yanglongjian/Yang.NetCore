using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("系统配置", "系统管理", Module = "Admin", OrderCode = 11)]
    public class SettingController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public SettingController(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read()
        {
            var row = await _repository.Queryable<Setting>().FirstAsync();
            if (row.IsNull()) return new AjaxResult(AjaxResultType.NoFound, "系统配置未初始化");
            return new AjaxResult(AjaxResultType.Success, row);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(SettingInfo dto)
        {
            var row = await _repository.Queryable<Setting>().FirstAsync();
            if (row.IsNull()) return new AjaxResult(AjaxResultType.NoFound, "系统配置未初始化");
            row.SettingInfo = dto.ToJson();
            var i = await _repository.Update(row, "SettingInfo");
            return new AjaxResult(AjaxResultType.Success, i);
        }

    }
}



