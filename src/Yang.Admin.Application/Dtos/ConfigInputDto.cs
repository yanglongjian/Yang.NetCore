using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 系统配置信息
    /// </summary>
    public class ConfigInputDto
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string Title { get; set; }
      
        /// <summary>
        /// 系统图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 是否使用谷歌验证码
        /// </summary>
        public bool IsUseGoogle { get; set; }


    }
}


