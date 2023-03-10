using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;

namespace Yang.Core
{
    /// <summary>
    /// IP扩展
    /// </summary>
    public class IPExtension
    {
        /// <summary>
        /// 获取服务器IP地址
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetWanIp()
        {
            string ip = string.Empty;
            string url = "http://www.net.cn/static/customercare/yourip.asp";
            string html = await url.GetAsync().Result.GetStringAsync();
            if (!string.IsNullOrEmpty(html))
                ip = Resove(html, "<h2>", "</h2>");
            return ip;
        }


        /// <summary>
        /// 获取局域网IP
        /// </summary>
        /// <returns></returns>
        public static string GetLanIp()
        {
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress.ToString();
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 通过标签前缀部分和后缀部分从HTML获取内容
        /// </summary>
        /// <param name="html"></param>
        /// <param name="prefix"></param>
        /// <param name="subfix"></param>
        /// <returns></returns>
        private static string Resove(string html, string prefix, string subfix)
        {
            int inl = html.IndexOf(prefix);
            if (inl == -1)
            {
                return null;
            }
            inl += prefix.Length;
            int inl2 = html.IndexOf(subfix, inl);
            string s = html[inl..inl2];
            return s;
        }


        /// <summary>
        /// 获取IP归属地
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static async Task<string> GetIpLocation(string ipAddress)
        {
            var url = "http://whois.pconline.com.cn/ip.jsp?ip=" + ipAddress;
            var result = await url.GetAsync().Result.GetStringAsync();
            string ipLocation = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                var resultArr = result.Split(' ');
                ipLocation = resultArr[0].Replace("省", "  ").Replace("市", "");
                ipLocation = ipLocation.Trim();
            }
            if (string.IsNullOrWhiteSpace(ipLocation)) ipLocation = "局域网";
            return ipLocation;
        }

        /// <summary>
        /// 把IP地址转换为Long型数字
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        public static long GetIpNum(string ipAddress)
        {
            string[] ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);
            long ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
        }


        /// <summary>
        /// 是否在IP范围内
        /// </summary>
        /// <param name="userIp"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsWhiteIp(long userIp, long begin, long end)
        {
            return (userIp >= begin) && (userIp <= end);
        }
    }
}



