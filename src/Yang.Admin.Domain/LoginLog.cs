using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 登录日志信息
    /// </summary>
    [SugarTable("Sys_LoginLog", "登录日志信息")]
    public class LoginLog : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Required]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(ColumnDescription = "用户编号")]
        public int UserId { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        [SugarColumn(ColumnDescription = "设备IP")]
        public string ClientIp { get; set; }
        /// <summary>
        /// 用户代理
        /// </summary>
        [SugarColumn(ColumnDescription = "用户代理")]
        public string UserAgent { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        [SugarColumn(ColumnDescription = "登录时间")]
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 登出时间
        /// </summary>
        [SugarColumn(ColumnDescription = "登出时间")]
        public DateTime? LogoutTime { get; set; }

    }
}



