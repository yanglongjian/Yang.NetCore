using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;

namespace CMS.Web.Entry.ViewModels
{
    public class NewsDto
    {
        /// <summary>
        /// 是否登录
        /// </summary>
        public int IsLogin { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 站点信息
        /// </summary>
        public Site Site { get; set; }
        /// <summary>
        /// 新闻信息
        /// </summary>
        public News News { get; set; }
        /// <summary>
        /// 栏目列表
        /// </summary>
        public List<Column> ColumnList { get; set; }
        /// <summary>
        /// 热门列表
        /// </summary>
        public List<NewsOutputDto> HotList { get; set; }
    }
}

