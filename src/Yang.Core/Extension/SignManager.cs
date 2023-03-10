using Furion;
using Furion.DataEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Core.Extension
{
    /// <summary>
    /// 签名管理器
    /// </summary>
    public class SignManager
    {
        /// <summary>
        /// 参数校验
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="ts"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool ParamVerify(string sign, long ts, Object p)
        {
            //判断是否需要签名
            bool isUserSign = App.Configuration["IsUserSign"].ToBool();
            if (!isUserSign) return true;

            try
            {
                var propertys = p.GetType().GetProperties();
                var orderPropertys = propertys.OrderBy(p => p.Name); //ASCII码从小到大排序(字典序)

                string temp = "";
                foreach (var item in orderPropertys)
                {
                    string name = item.Name;
                    object oValue = item.GetValue(p);
                    string value = "";
                    if (oValue != null)//如果参数不为空则拼接参数
                    {
                        value = oValue.ToString();
                        temp += name + "=" + value + "&";
                    }

                }

                string secretKey = App.Configuration["SerectKey"];
                temp += "key=" + secretKey;

                string md = MD5Encryption.Encrypt(temp);
                if (sign != "" && sign.ToUpper() == md.ToUpper())
                {
                    //签名验证成功
                    return true;
                }
                else
                {
                    //签名失败
                    return false;
                }

            }
            catch
            {
                //签名异常信息
                return false;

            }

        }
    }
}


