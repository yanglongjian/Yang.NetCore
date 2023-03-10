using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;

namespace CMS.Web.Entry.ViewModels
{
    public class ColumnDto
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
        /// 栏目信息
        /// </summary>
        public Column Column { get; set; }
        /// <summary>
        /// 栏目列表
        /// </summary>
        public List<Column> ColumnList { get; set; }
        /// <summary>
        /// 新闻列表
        /// </summary>
        public List<NewsOutputDto> NewsList { get; set; }
       
        /// <summary>
        /// 热门列表
        /// </summary>
        public List<NewsOutputDto> HotList { get; set; }



        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 总分页数
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// 分页数组
        /// </summary>
        public List<PageDto> PageList { get; set; }
    }
}

