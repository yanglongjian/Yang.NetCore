using SqlSugar;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 模块角色信息
    /// </summary>
    [SugarTable("Sys_ModuleRole", "模块角色信息")]
    public class ModuleRole : IEntity<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        [SugarColumn(ColumnDescription = "模块编号")]
        public string ModuleId { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        [SugarColumn(ColumnDescription = "角色编号")]
        public int RoleId { get; set; }
    }
}



