using SqlSugar;
using System;
using Yang.Core;


namespace Yang.Admin.Domain
{
    /// <summary>
    /// 审计日志
    /// </summary>
    [SugarTable("Sys_AuditLog", "审计日志")]
    public class AuditLog : IEntity<int>, ICreatedAudit<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnDescription = "表名")]
        public string TableName { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        [SugarColumn(ColumnDescription = "操作类型")]
        public DiffType DiffType { get; set; }

        /// <summary>
        /// 操作前记录
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "操作前记录")]
        public string BeforeData { get; set; }

        /// <summary>
        /// 操作后记录
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "操作后记录")]
        public string AfterData { get; set; }

        /// <summary>
        /// 执行Sql
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "执行Sql")]
        public string Sql { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "参数")]
        public string Parameter { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        [SugarColumn(ColumnDescription = "耗时")]
        public double TotalMilliseconds { get; set; }

        /// <summary>
        /// 业务参数
        /// </summary>
        [SugarColumn(ColumnDescription = "业务参数")]
        public string BusinessData { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(ColumnDescription = "创建人")]
        public int? CreatedId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }

    }
}


