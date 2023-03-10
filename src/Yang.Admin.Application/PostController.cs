using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Admin.Application
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("岗位管理", "系统管理", Module = "Admin", OrderCode = 4)]
    public class PostController : IDynamicApiController
    {
        private readonly IRepository _repository;
        /// <summary>
        /// 
        /// </summary>
        public PostController(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<AjaxResult> Read(PageRequest request)
        {
            var (rows, total) = await _repository.GetPageList<Post>(request, "OrderCode");
            return new AjaxResult(AjaxResultType.Success, new { rows, total });
        }


        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("获取岗位列表")]
        public virtual async Task<AjaxResult> GetPosts()
        {
            var rows = await _repository.Queryable<Post>().Where(r => r.Status == 0).Select(r => new
            {
                Label = r.Name,
                Value = r.Id
            }).ToListAsync();
            return new AjaxResult(AjaxResultType.Success, rows);
        }



        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ModuleInfo("新增")]
        public virtual async Task<AjaxResult> Create(PostInputDto dto)
        {
            await _repository.InsertDto<Post, PostInputDto>(dto);
            return new AjaxResult(AjaxResultType.Success, 1);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<AjaxResult> Update(PostInputDto dto)
        {
            var i = await _repository.UpdateDto<Post, PostInputDto>(dto);
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
            var i = await _repository.Delete<Post>(r => ids.Contains(r.Id));
            return new AjaxResult(AjaxResultType.Success, i);
        }
    }
}


