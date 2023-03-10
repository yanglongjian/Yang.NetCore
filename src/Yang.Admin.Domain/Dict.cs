using SqlSugar;
using System;
using System.Collections.Generic;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [SugarTable("Sys_Dict", "数据字典")]
    public class Dict : IEntity<int>, ICreatedTime
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [SugarColumn(ColumnDescription = "父节点")]
        public int ParentId { get; set; }

        /// <summary>
        /// 键值
        /// </summary>
        [SugarColumn(ColumnDescription = "键名")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDescription = "名称")]
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


        /// <summary>
        /// 键值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int Key
        {
            get
            {
                return Id;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Title
        {
            get
            {
                return Name;
            }
        }


        /// <summary>
        /// 树结构子节点
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Dict> Children { get; set; }

    }
}


