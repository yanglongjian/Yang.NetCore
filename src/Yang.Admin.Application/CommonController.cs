using Furion;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yang.Admin.Domain;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 通用
    /// </summary>
    [NonUnify]
    [LoggedIn]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("通用", "系统管理", Module = "Admin")]
    public class CommonController : IDynamicApiController
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 
        /// </summary>
        public CommonController(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ModuleInfo("获取验证码")]
        public virtual AjaxResult VerifyCode()
        {
            ValidateCoder coder = new()
            {
                RandomColor = true,
                RandomItalic = true,
                RandomLineCount = 7,
                RandomPointPercent = 10,
                RandomPosition = true
            };
            var bitmap = coder.CreateImage(4, out string code, ValidateCodeType.NumberAndLetter);
            //设置验证码缓存
            var codeId = Id.NextId();
            string key = $"{AdminConstants.VerifyCodeKeyPrefix}_{codeId}";
            _cache.Set(key, code, DateTime.Now.AddMinutes(2));

            using MemoryStream ms = new();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.ToArray();
            string str = $"data:image/png;base64,{bytes.ToBase64String()}{AdminConstants.Separator}{codeId}";

            return new AjaxResult(AjaxResultType.Success, new
            {
                VerifyCode = str.ToBase64String()
            });
        }

        /// <summary>
        /// 验证验证码的有效性
        /// </summary>
        /// <param name="id">验证码编号</param>
        /// <param name="code">验证码字符串</param>
        /// <returns>是否无效</returns>
        [AllowAnonymous]
        [ModuleInfo("验证验证码的有效性")]
        public virtual AjaxResult CheckVerifyCode(string id, string code)
        {
            var result = ValidateCoder.CheckVerifyCode(AdminConstants.VerifyCodeKeyPrefix, id, code, false);

            return new AjaxResult(AjaxResultType.Success, result);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        [AllowAnonymous]
        [ModuleInfo("上传图片")]
        public virtual async Task<AjaxResult> UploadImage(IFormFile file)
        {
            var _accessor = App.GetRequiredService<IHttpContextAccessor>();

            var env = App.GetService<IWebHostEnvironment>();
            if (env.WebRootPath.IsEmpty()) return new AjaxResult(AjaxResultType.Error, "未创建网站根目录");

            string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:MMddHHmmssff}{Path.GetExtension(file.FileName)}";
            string dir = Path.Combine(env.WebRootPath, "upload-files");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (FileStream fs = new(dir + $"\\{fileName}", FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            return new AjaxResult(AjaxResultType.Success, new
            {
                ImageUrl = $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}/upload-files/{fileName}"
            });
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ModuleInfo("上传文件")]
        public virtual async Task<AjaxResult> UploadFile()
        {
            var _accessor = App.GetRequiredService<IHttpContextAccessor>();

            var file = _accessor.HttpContext.Request.Form.Files[0];

            string signal = _accessor.HttpContext.Request.Form["signal"].ToString();
            string name = _accessor.HttpContext.Request.Form["name"].ToString();
            long size = _accessor.HttpContext.Request.Form["size"].ToLong();
            var chunks = _accessor.HttpContext.Request.Form["chunks"].ToInt();
            var chunk = _accessor.HttpContext.Request.Form["chunk"].ToInt();


            var _env = App.GetService<IWebHostEnvironment>();
            if (_env.WebRootPath.IsEmpty()) return new AjaxResult(AjaxResultType.Warning, "未建立文件存储根目录文件夹");

            string dir = Path.Combine(_env.WebRootPath, "upload-temp");
            string temporary = Path.Combine(dir, signal);//临时保存分块的目录
            try
            {
                if (!Directory.Exists(temporary))
                    Directory.CreateDirectory(temporary);
                string filePath = Path.Combine(temporary, chunk.ToString());
                if (!Convert.IsDBNull(file))
                {
                    await Task.Run(() =>
                    {
                        FileStream fs = new(filePath, FileMode.Create);
                        file.CopyTo(fs);
                        fs.Close();
                    });
                }


                if (chunks == chunk)
                {
                    await FileMerge(name, temporary);
                }

                return new AjaxResult(AjaxResultType.Success);
            }
            catch
            {
                Directory.Delete(temporary);//删除文件夹
            }
            return new AjaxResult(AjaxResultType.Success);
        }


        /// <summary>
        /// 文件合并
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="temporary"></param>
        /// <returns></returns>
        private static async Task<bool> FileMerge(string fileName, string temporary)
        {
            bool ok = false;
            try
            {
                string fileExt = Path.GetExtension(fileName);//获取文件后缀
                var files = Directory.GetFiles(temporary);//获得下面的所有文件
                var finalPath = Path.Combine(temporary, DateTime.Now.ToString("yyMMddHHmmss") + fileExt);//最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                var fs = new FileStream(finalPath, FileMode.Create);
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                {
                    var bytes = File.ReadAllBytes(part);
                    await fs.WriteAsync(bytes.AsMemory(0, bytes.Length));
                    bytes = null;
                    File.Delete(part);//删除分块
                }
                fs.Close();
                Directory.Delete(temporary);//删除文件夹
                ok = true;
            }
            catch
            {
            }
            return ok;
        }
    }
}



