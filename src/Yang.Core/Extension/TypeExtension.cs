using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Yang.Core
{
    /// <summary>
    /// 强类型转换扩展
    /// </summary>
    public static class TypeExtension
    {
        #region 对象判断可空 返回bool
        /// <summary>
        /// 字符串为空
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string strValue)
        {
            return string.IsNullOrEmpty(strValue);
        }

        /// <summary>
        /// 字符串不为空
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string strValue)
        {
            return !string.IsNullOrEmpty(strValue);
        }

        /// <summary>
        /// 对象为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// 对象不为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object obj)
        {
            return !(obj == null);
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool Equal<T>(this T x, T y)
        {
            return ((IComparable)(x)).CompareTo(y) == 0;
        }

        /// <summary>
        /// 判断类型是否可空
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        #endregion

        #region 数据强类型转换

        /// <summary>
        /// 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int ToInt(this object data)
        {
            if (data == null)
                return 0;
            var success = int.TryParse(data.ToString(), out int result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转64位长类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long ToLong(this object data)
        {
            if (data == null)
                return 0;
            var success = long.TryParse(data.ToString(), out long result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt64(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static double ToDouble(this object data, int digits = 2)
        {
            return Math.Round(DoubleParse(data), digits);
        }

        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        private static double DoubleParse(this object data)
        {
            if (data == null)
                return 0;
            return double.TryParse(data.ToString(), out double result) ? result : 0;
        }

        /// <summary>
        /// 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal ToDecimal(this object data, int digits = 2)
        {
            return Math.Round(DecimalParse(data), digits);
        }

        /// <summary>
        /// 转换为高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        private static decimal DecimalParse(this object data)
        {
            if (data == null)
                return 0;
            return decimal.TryParse(data.ToString(), out decimal result) ? result : 0;
        }


        /// <summary>
        /// 转Bool
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ToBool(this object data)
        {
            if (data.IsNull())
            {
                return false;
            }
            else
            {
                if (bool.TryParse(data.ToString(), out bool result))
                {
                    return result;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 转时间
        /// </summary>
        /// <param name="data">数据</param>
        public static DateTime ToDateTime(this object data)
        {
            if (data == null) return DateTime.Parse("1970-01-01 00:00:00");
            bool isValid = DateTime.TryParse(data.ToString(), out DateTime result);
            if (isValid)
                return result;
            return DateTime.Parse("1970-01-01 00:00:00");
        }


        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ParseToDateTime(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DateTime.MinValue;
            }
            if (str.Contains("-") || str.Contains("/"))
            {
                return DateTime.Parse(str);
            }
            else
            {
                int length = str.Length;
                return length switch
                {
                    4 => DateTime.ParseExact(str, "yyyy", System.Globalization.CultureInfo.CurrentCulture),
                    6 => DateTime.ParseExact(str, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture),
                    8 => DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                    10 => DateTime.ParseExact(str, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture),
                    12 => DateTime.ParseExact(str, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture),
                    14 => DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture),
                    _ => DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture),
                };
            }
        }



        /// <summary>
        /// 转为时间戳
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime data)
        {
            return (data.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }


        /// <summary>
        /// String数组转Int数组
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static int[] ToIntArr(this string[] strs)
        {
            return Array.ConvertAll(strs, int.Parse);
        }


        #endregion

        #region  JSON
        /// <summary>
        /// 转json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, string datetimeformats = "yyyy-MM-dd HH:mm:ss")
        {
            if (obj == null) return "";
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }

        /// <summary>
        /// json转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default : JsonConvert.DeserializeObject<T>(Json);
        }

        #endregion

        #region Property 对象属性

        /// <summary>
        /// 获取类型的所有属性值
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string[] GetPropertyName(this Type t)
        {
            List<string> result = new();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                result.Add(pi.Name);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 取一个类指定的属性值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object info, string field)
        {
            if (info == null) return null;
            Type t = info.GetType();
            IEnumerable<PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            return property.First().GetValue(info, null);
        }

        #endregion

        #region  数组集合转字符串

        /// <summary>
        /// []数组转换为Base64字符串
        /// </summary>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将字符串转换为Base64字符串，默认编码为UTF8
        /// </summary>
        /// <param name="source">正常的字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>Base64字符串</returns>
        public static string ToBase64String(this string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(encoding.GetBytes(source));
        }


        /// <summary>
        /// 集合转字符串
        /// </summary>
        /// <param name="collection"> </param>
        /// <param name="separator"> 分隔符默认为逗号 </param>
        /// <returns> </returns>
        public static string ExpandAndToString<T>(this IEnumerable<T> collection, string separator = ",")
        {
            return collection.ExpandAndToString(item => item?.ToString() ?? string.Empty, separator);
        }


        private static string ExpandAndToString<T>(this IEnumerable<T> collection, Func<T, string> itemFormatFunc, string separator = ",")
        {
            collection = collection as IList<T> ?? collection.ToList();
            if (!collection.Any())
            {
                return string.Empty;
            }

            StringBuilder sb = new();
            int i = 0;
            int count = collection.Count();
            foreach (T item in collection)
            {
                if (i == count - 1)
                {
                    sb.Append(itemFormatFunc(item));
                }
                else
                {
                    sb.Append(itemFormatFunc(item) + separator);
                }

                i++;
            }

            return sb.ToString();
        }
        #endregion



        /// <summary>
        /// 将时间转换为JS时间格式(Date.getTime())
        /// </summary>
        public static string ToJsGetTime(this DateTime dateTime, bool milsec = true)
        {
            DateTime utc = dateTime.ToUniversalTime();
            TimeSpan span = utc.Subtract(new DateTime(1970, 1, 1));
            return Math.Round(milsec ? span.TotalMilliseconds : span.TotalSeconds).ToString();
        }

    }
}



