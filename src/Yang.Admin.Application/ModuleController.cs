namespace Yang.Application.Admin
{
    /// <summary>
    /// 模块信息
    /// </summary>
    [ModuleInfo("模块信息", "系统管理", Module = "Admin", OrderCode = 6)]
    public class ModuleController : IDynamicApiController
    {
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 
        /// </summary>
        public ModuleController(IMemoryCache cache)
        {
            _cache = cache;
        }



        /// <summary>
        /// 读取模块功能列表
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取")]
        public virtual List<Module> Read()
        {
            //读取缓存
            return _cache.Get<Module[]>(Constants.SystemModule).OrderBy(r => r.OrderCode).ToList();
        }

        /// <summary>
        /// 读取功能树
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("读取功能树")]
        public virtual async Task<List<Module>> ReadTree()
        {
            return await _cache.GetOrCreateAsync(Constants.SystemModuleTree, async item =>
            {
                return await DbContext.Instance.Queryable<Module>().ToTreeAsync(it => it.Children, it => it.ParentId, "");
            });

        }

    }
}



