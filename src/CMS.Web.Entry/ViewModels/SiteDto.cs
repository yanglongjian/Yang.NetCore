using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;

namespace CMS.Web.Entry.ViewModels
{
    /// <summary>
    /// 首页Dto
    /// </summary>
    public class SiteDto
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
        /// 栏目列表
        /// </summary>
        public List<Column> ColumnList { get; set; }

        /// <summary>
        /// 栏目新闻列表
        /// </summary>
        public List<ColumnNews> ColumnNewsList { get; set; } = new List<ColumnNews>();

        /// <summary>
        /// 热门列表
        /// </summary>
        public List<NewsOutputDto> HotList { get; set; }
    }

    /// <summary>
    /// 栏目新闻信息
    /// </summary>
    public class ColumnNews
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string Link { get; set; }
        public List<NewsOutputDto> NewsList { get; set; }
    }
}

