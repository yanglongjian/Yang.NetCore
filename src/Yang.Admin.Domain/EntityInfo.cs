using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 数据信息
    /// </summary>
    [SugarTable("Sys_EntityInfo", "数据信息")]
    public class EntityInfo : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDescription = "名称")]
        public string Name { get; set; }
        /// <summary>
        /// 实体类型名称
        /// </summary>
        [SugarColumn(ColumnDescription = "实体类型名称")]
        public string TypeName { get; set; }

        /// <summary>
        /// 数据库表名
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "数据库表名", UniqueGroupNameList = new[] { "index_entity_tablename" })]
        public string TableName { get; set; }

        /// <summary>
        /// 数据审计
        /// </summary>
        [SugarColumn(ColumnDescription = "数据审计")]
        public bool IsAudit { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }


    }
}



