using Furion;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Yang.Core
{
    /// <summary>
    /// 登录令牌信息
    /// </summary>
    public class ClaimsExtension
    {

        /// <summary>
        /// 登录令牌信息
        /// </summary>
        public static TokenInfo User
        {
            get
            {
                var claims = App.User.Claims.ToList();

                Dictionary<string, object> dict = new();

                claims.ForEach(item =>
                {
                    dict.Add(item.Type, item.Value);
                });
                return JsonConvert.DeserializeObject<TokenInfo>(JsonConvert.SerializeObject(dict));
            }
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public static int UserId
        {
            get
            {
                return (App.User?.FindFirstValue("UserId")).ToInt();
            }
        }
    }
}


