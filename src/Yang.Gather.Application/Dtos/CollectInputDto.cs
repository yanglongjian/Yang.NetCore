using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Gather.Domain;

namespace Yang.Gather.Application.Dtos
{
    /// <summary>
    /// 采集输入Dto
    /// </summary>
    public class CollectInputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 采集名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// 采集类型 0-html 1-ajax
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 采集类型 0-新闻 1-栏目
        /// </summary>
        public int CollectType { get; set; }

        /// <summary>
        /// 采集地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 是否累加
        /// </summary>
        public bool IsAdd { get; set; }


        /// <summary>
        /// 执行间隔
        /// </summary>
        public int Interval { get; set; }


        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextTime { get; set; }

        /// <summary>
        /// 采集数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 状态 0-启用 1-禁用
        /// </summary>
        public int Status { get; set; }


        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublish { get; set; }


        /// <summary>
        /// 列表开始
        /// </summary>
        public string Begin { get; set; }

        /// <summary>
        /// 列表结束
        /// </summary>
        public string End { get; set; }


        /// <summary>
        /// 配置列表
        /// </summary>
        public List<CollectConfig> ConfigList { get; set; }
    }
}

