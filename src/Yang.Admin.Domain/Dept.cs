using SqlSugar;
using System;
using System.Collections.Generic;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 部门信息
    /// </summary>
    [SugarTable("Sys_Dept", "部门信息")]
    public class Dept : IEntity<int>, ICreatedTime
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
        /// 部门名称
        /// </summary>
        [SugarColumn(ColumnDescription = "部门名称")]
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
        public List<Dept> Children { get; set; }
    }
}


