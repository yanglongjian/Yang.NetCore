using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Yang.Admin.Application.Dtos;
using Yang.Core;

namespace Yang.Application.Admin
{
    /// <summary>
    /// 服务器信息
    /// </summary>
    [NonUnify]
    [ApiDescriptionSettings(Module = "Admin")]
    [ModuleInfo("服务器信息", "系统管理", Module = "Admin", OrderCode = 10)]
    public class ServerController : IDynamicApiController
    {
        /// <summary>
        /// 获取服务器状态
        /// </summary>
        [ModuleInfo("获取服务器状态")]
        public virtual AjaxResult GetServerStatus()
        {
            ServerMemory memory = GetMemoryMetrics();
            var result = new ServerStatus
            {
                TotalRAM = Math.Ceiling(memory.Total / 1024).ToString() + " GB",
                RAMRate = Math.Ceiling(100 * memory.Used / memory.Total),
                CPURate = Math.Ceiling(GetCPURate().ToDouble()),
                RunTime = GetRunTime()
            };

            return new AjaxResult(AjaxResultType.Success, result);
        }

        /// <summary>
        /// 获取服务器基本信息
        /// </summary>
        /// <returns></returns>
        [ModuleInfo("获取服务器基本信息")]
        public virtual async Task<AjaxResult> GetServerInfo()
        {
            var ip = await IPExtension.GetWanIp();

            var result = new ServerInfo()
            {
                Ip = ip,
                IpLocation = await IPExtension.GetIpLocation(ip),
                LanIp = IPExtension.GetLanIp(),
                ServerName = Environment.MachineName,
                SystemOs = RuntimeInformation.OSDescription,
                OsArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                ProcessorCount = Environment.ProcessorCount.ToString(),
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                RamUse = ((double)System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB",
                StartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm")
            };

            return new AjaxResult(AjaxResultType.Success, result);
        }



        #region 服务器信息计算
        /// <summary>
        /// 判断服务器平台
        /// </summary>
        /// <returns></returns>
        private static bool IsLinux()
        {
            //判断是否linux平台
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }


        /// <summary>
        /// 获取内存使用状况
        /// </summary>
        /// <returns></returns>
        private static ServerMemory GetMemoryMetrics()
        {
            if (IsLinux())
            {
                string output = ShellExtension.Bash("free -m");
                var lines = output.Split("\n");
                var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var metrics = new ServerMemory
                {
                    Total = double.Parse(memory[1]),
                    Used = double.Parse(memory[2]),
                    Free = double.Parse(memory[3])
                };

                return metrics;
            }
            else
            {
                string output = ShellExtension.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
                var lines = output.Trim().Split("\n");
                var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var metrics = new ServerMemory
                {
                    Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0),
                    Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0)
                };
                metrics.Used = metrics.Total - metrics.Free;

                return metrics;
            }

        }


        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        private static string GetCPURate()
        {
            if (IsLinux())
            {
                string output = ShellExtension.Bash("top -b -n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
                return output.Trim();
            }
            else
            {
                string output = ShellExtension.Cmd("wmic", "cpu get LoadPercentage");
                return output.Replace("LoadPercentage", string.Empty).Trim();
            }
        }

        /// <summary>
        /// 获取运行时间
        /// </summary>
        /// <returns></returns>
        private static string GetRunTime()
        {
            string runTime = string.Empty;
            if (IsLinux())
            {
                string output = ShellExtension.Bash("uptime -s");
                output = output.Trim();
                runTime = FormatTime((DateTime.Now - output.ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ToLong());
            }
            else
            {
                string output = ShellExtension.Cmd("wmic", "OS get LastBootUpTime/Value");
                string[] outputArr = output.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (outputArr.Length == 2)
                {
                    runTime = FormatTime((DateTime.Now - outputArr[1].Split('.')[0].ParseToDateTime()).TotalMilliseconds.ToString().Split('.')[0].ToLong());
                }
            }
            return runTime;
        }

        /// <summary>
        ///毫秒转天时分秒
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string FormatTime(long ms)
        {
            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;
            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            // long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;
            string sDay = day < 10 ? "0" + day : "" + day; //天
            string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
            string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            string sSecond = second < 10 ? "0" + second : "" + second;//秒
            //string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
            //sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;
            return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
        }
        #endregion

    }
}



