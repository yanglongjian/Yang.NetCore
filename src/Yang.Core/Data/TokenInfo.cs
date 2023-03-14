using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Core
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 是否系统账户
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; } = UserType.Admin;
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; } = DateTime.Now;
    }

    public enum UserType
    {
        /// <summary>
        /// Admin管理后台
        /// </summary>
        [Description("Admin")] Admin = 1,
        /// <summary>
        /// App
        /// </summary>
        [Description("App")] App = 2
    }
}


