using SqlSugar;
using System;
using System.Collections.Generic;
using Yang.Core;

namespace Yang.Gather.Domain
{
    /// <summary>
    /// 采集管理
    /// </summary>
    [SugarTable("App_Collect", "采集管理")]
    public class Collect : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 采集名称
        /// </summary>
        [SugarColumn(ColumnDescription = "采集名称")]
        public string Name { get; set; }

        /// <summary>
        /// 栏目编号
        /// </summary>
        [SugarColumn(ColumnDescription = "栏目编号")]
        public int ColumnId { get; set; }

        /// <summary>
        /// 采集类型 0-html 1-ajax 2-无头浏览器
        /// </summary>
        [SugarColumn(ColumnDescription = "采集类型")]
        public int Type { get; set; }

        /// <summary>
        /// 采集类型 0-新闻 1-栏目
        /// </summary>
        [SugarColumn(ColumnDescription = "采集类型")]
        public int CollectType { get; set; }

        /// <summary>
        /// 采集地址
        /// </summary>
        [SugarColumn(ColumnDescription = "采集地址")]
        public string Url { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [SugarColumn(ColumnDescription = "参数")]
        public string Parameter { get; set; }

        /// <summary>
        /// 是否累加
        /// </summary>
        [SugarColumn(ColumnDescription = "是否累加")]
        public bool IsAdd { get; set; }

        /// <summary>
        /// 采集数
        /// </summary>
        [SugarColumn(ColumnDescription = "采集数")]
        public int Count { get; set; }


        /// <summary>
        /// 执行间隔
        /// </summary>
        [SugarColumn(ColumnDescription = "执行间隔")]
        public int Interval { get; set; }


        /// <summary>
        /// 下次执行时间
        /// </summary>
        [SugarColumn(ColumnDescription = "下次执行时间")]
        public DateTime? NextTime { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 状态 0-启用 1-禁用
        /// </summary>
        [SugarColumn(ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [SugarColumn(ColumnDescription = "是否发布")]
        public bool IsPublish { get; set; }


        /// <summary>
        /// 列表开始
        /// </summary>
        [SugarColumn(ColumnDescription = "列表开始")]
        public string Begin { get; set; }

        /// <summary>
        /// 列表结束
        /// </summary>
        [SugarColumn(ColumnDescription = "列表结束")]
        public string End { get; set; }


        /// <summary>
        /// 配置信息
        /// </summary>    
        [Navigate(NavigateType.OneToMany, nameof(CollectConfig.CollectId))]
        public List<CollectConfig> ConfigList { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public string ColumnName { get; set; }
    }

}

