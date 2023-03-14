namespace Yang.Admin.Application
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [ModuleInfo("数据字典", "系统管理", Module = "Admin", OrderCode = 5)]
    public class DictController : IDynamicApiController
    {

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual async Task<List<Dict>> Read()
        {
            return await DbContext.Instance.Queryable<Dict>().OrderBy(r => r.OrderCode).ToTreeAsync(it => it.Children, it => it.ParentId, 0);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ModuleInfo("新增")]
        public virtual async Task<int> Create(Dict entity)
        {
            return await DbContext.Instance.Insertable(entity).ExecuteCommandAsync();
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("更新")]
        public virtual async Task<int> Update(Dict entity)
        {
            return await DbContext.Instance.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [ModuleInfo("删除")]
        public virtual async Task<int> Delete([FromBody] int[] ids)
        {
            return await DbContext.Instance.Deleteable<Dict>().Where(r => ids.Contains(r.Id)).ExecuteCommandAsync();
        }
    }
}


