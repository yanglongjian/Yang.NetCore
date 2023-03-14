namespace Yang.Application.Admin
{
    /// <summary>
    /// 通用
    /// </summary>
    [LoggedIn]
    [ModuleInfo("通用", "系统管理", Module = "Admin")]
    public class CommonController : IDynamicApiController
    {
        //文件夹
        private readonly string fileFolder = "upload-files";
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        public CommonController(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        [AllowAnonymous]
        [ModuleInfo("上传图片")]
        public virtual async Task<string> UploadImage(IFormFile file)
        {
            var _accessor = App.GetRequiredService<IHttpContextAccessor>();
            var env = App.GetService<IWebHostEnvironment>();
            if (env.WebRootPath.IsEmpty())
                throw Oops.Bah("未创建网站根目录");
            string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:MMddHHmmssff}{Path.GetExtension(file.FileName)}";
            string dir = Path.Combine(env.WebRootPath, fileFolder);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (FileStream fs = new(dir + $"\\{fileName}", FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            return $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}/{fileFolder}/{fileName}";
        }
    }
}



