using SqlSugar;
using System;
using Yang.Core;
namespace Yang.Admin.Domain
{
    /// <summary>
    /// 岗位信息
    /// </summary>
    [SugarTable("Sys_Post", "岗位信息")]
    public class Post : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [SugarColumn(ColumnDescription = "岗位编码")]
        public string Code { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [SugarColumn(ColumnDescription = "岗位名称")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnDescription = "排序")]
        public int OrderCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDescription = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }
    }
}


