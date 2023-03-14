using SqlSugar;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 用户详细信息
    /// </summary>
    [SugarTable("Sys_UserDetail", "用户详细信息")]
    public class UserDetail : IEntity<int>
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
        /// 密码
        /// </summary>
        [SugarColumn(ColumnDescription = "密码")]
        public string Password { get; set; }


        /// <summary>
        /// 注册IP
        /// </summary>
        [SugarColumn(ColumnDescription = "注册Ip")]
        public string RegisterIp { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        [SugarColumn(ColumnDescription = "用户代理")]
        public string UserAgent { get; set; }
    }
}



