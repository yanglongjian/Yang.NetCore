using Furion;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Yang.Web.Pack
{
    public class MultiPack
    {
        /// <summary>
        /// 初始化多语言内存
        /// </summary>
        public static void InitLanguage()
        {
            var _cache = App.GetService<IMemoryCache>();

            DirectoryInfo root = new(Path.Combine(Directory.GetCurrentDirectory(), "Lang"));
            var files = root.GetFiles();
            foreach (var item in files)
            {
                //乱码处理
                using var file = new StreamReader(item.FullName, Encoding.GetEncoding("gb2312"));
                using var reader = new JsonTextReader(file);

                var langObj = (JObject)JToken.ReadFrom(reader);
                string key = item.Name.Replace(item.Extension, "");
                _cache.Set(key, langObj);
            }
        }
    }
}

