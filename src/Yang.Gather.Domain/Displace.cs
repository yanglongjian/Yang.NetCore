using SqlSugar;
using System;
using System.Collections.Generic;
using Yang.Core;

namespace Yang.Gather.Domain
{
    /// <summary>
    /// 替换管理
    /// </summary>
    [SugarTable("App_Displace", "替换词汇")]
    public class Displace : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 原词汇
        /// </summary>
        [SugarColumn(ColumnDescription = "原词汇")]
        public string Word { get; set; }

        /// <summary>
        /// 新词汇
        /// </summary>
        [SugarColumn(ColumnDescription = "新词汇")]
        public string NewWord { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }
    }

}


