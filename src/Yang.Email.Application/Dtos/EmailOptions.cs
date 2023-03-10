using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Email.Application.Dtos
{
    /// <summary>
    /// 邮件发送方配置
    /// </summary>
    public class EmailOptions
    {
        /// <summary>
        /// 邮件发送服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否SSL
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 发送方显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 发送方用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 发送方密码
        /// </summary>
        public string Password { get; set; }
    }
}

