using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yang.Core;


namespace Yang.Admin.Domain
{
    /// <summary>
    /// 角色选择部门信息
    /// </summary>
    [SugarTable("Sys_RoleDept", "角色选择部门信息")]
    public class RoleDept : IEntity<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "角色编号")]
        public int RoleId { get; set; }

        /// <summary>
        /// 部门编号
        /// </summary>
        [Required]
        [SugarColumn(ColumnDescription = "部门编号")]
        public int DeptId { get; set; }
    }
}


