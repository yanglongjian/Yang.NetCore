using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 服务器信息
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// 外网IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpLocation { get; set; }
        /// <summary>
        /// 局域网IP
        /// </summary>
        public string LanIp { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 服务器系统
        /// </summary>
        public string SystemOs { get; set; }
        /// <summary>
        /// 系统架构
        /// </summary>
        public string OsArchitecture { get; set; }
        /// <summary>
        /// CPU核心数
        /// </summary>
        public string ProcessorCount { get; set; }
        /// <summary>
        /// .net core版本
        /// </summary>
        public string FrameworkDescription { get; set; }
        /// <summary>
        /// 内存使用率
        /// </summary>
        public string RamUse { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public string StartTime { get; set; }
    }


    /// <summary>
    /// 服务器状态信息
    /// </summary>
    public class ServerStatus
    {
        /// <summary>
        /// CPU使用率
        /// </summary>
        public double CPURate { get; set; }
        /// <summary>
        /// 总内存
        /// </summary>
        public string TotalRAM { get; set; }
        /// <summary>
        /// 内存使用率
        /// </summary>
        public double RAMRate { get; set; }
        /// <summary>
        /// 系统运行时间
        /// </summary>
        public string RunTime { get; set; }
    }

    /// <summary>
    /// 服务器内存指标
    /// </summary>
    public class ServerMemory
    {
        /// <summary>
        /// 总内存
        /// </summary>
        public double Total { get; set; }
        /// <summary>
        /// 已使用
        /// </summary>
        public double Used { get; set; }
        /// <summary>
        /// 空闲内存
        /// </summary>
        public double Free { get; set; }
    }
}



