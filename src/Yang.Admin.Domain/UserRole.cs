using SqlSugar;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 用户角色信息
    /// </summary>
    [SugarTable("Sys_UserRole", "用户角色信息")]
    public class UserRole : IEntity<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(ColumnDescription = "用户编号")]
        public int UserId { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        [SugarColumn(ColumnDescription = "角色编号")]
        public int RoleId { get; set; }
    }
}



