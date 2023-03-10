using SqlSugar;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 模块用户信息
    /// </summary>
    [SugarTable("Sys_ModuleUser","模块用户信息")]
    public class ModuleUser : IEntity<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true,ColumnDescription ="编号")]
        public int Id { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        [SugarColumn(ColumnDescription = "模块编号")]
        public string ModuleId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(ColumnDescription = "用户编号")]
        public int UserId { get; set; }
    }
}



