using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 模块信息
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("模块信息", "系统管理", Module = "Admin", OrderCode = 6)]
    public class ModuleController : IDynamicApiController
    {

        private readonly IRepository _repository;
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 
        /// </summary>
        public ModuleController(IRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }



        /// <summary>
        /// 读取模块功能列表
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual AjaxResult Read()
        {
            //读取缓存
            var modules = _cache.Get<Module[]>(AdminConstants.SystemModuleKey);
            return new AjaxResult(AjaxResultType.Success, modules.OrderBy(r => r.OrderCode));
        }

        /// <summary>
        /// 读取功能树
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取功能树")]
        public virtual async Task<AjaxResult> ReadTree(PageRequest request)
        {
          
            if (request.FilterRules.Length == 0)
            {
                var tree = await _repository.Queryable<Module>().ToTreeAsync(it => it.Children, it => it.ParentId,"");
                return new AjaxResult(AjaxResultType.Success, tree);
            }
            else
            {
                var predicate = FilterHelper.GetExpression<Module>(request.FilterRules);
                var id = await _repository.Queryable<Module>().Where(predicate).Select(it => it.Id).FirstAsync();
                //将条件过滤后的ID传进ToTree
                var tree = await _repository.Queryable<Module>().ToTreeAsync(it => it.Children, it => it.ParentId, id);
                return new AjaxResult(AjaxResultType.Success, tree);
            }
        }


        private static List<Module> GetModuleTree(List<Module> modules, string parentId = "")
        {
            List<Module> nodes = new();
            foreach (var item in modules.Where(r => r.ParentId == parentId).OrderBy(r => r.OrderCode))
            {

                var node = new Module
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    Area = item.Area,
                    OrderCode = item.OrderCode,
                    AccessType = item.AccessType,
                };
                var children = GetModuleTree(modules, item.Id);
                if (children.Count > 0)
                {
                    node.Children = children;
                }

                nodes.Add(node);
            }
            return nodes;

        }

    }
}



