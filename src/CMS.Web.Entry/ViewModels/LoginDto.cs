using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Cms.Domain;

namespace CMS.Web.Entry.ViewModels
{
    public class LoginDto
    {
        /// <summary>
        /// 站点信息
        /// </summary>
        public Site Site { get; set; }
        /// <summary>
        /// 栏目列表
        /// </summary>
        public List<Column> ColumnList { get; set; }
    }
}

