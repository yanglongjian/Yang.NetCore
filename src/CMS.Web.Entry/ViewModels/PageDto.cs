using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Entry.ViewModels
{
    /// <summary>
    /// 分页组件
    /// </summary>
    public class PageDto
    {
        /// <summary>
        /// 页码
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Link { get; set; }
    }
}
