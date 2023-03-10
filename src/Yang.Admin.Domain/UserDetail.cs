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
        /// 注册IP
        /// </summary>
        [SugarColumn(ColumnDescription = "注册Ip")]
        public string RegisterIp { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        [SugarColumn(ColumnDescription = "用户代理")]
        public string UserAgent { get; set; }


        /// <summary>
        /// 是否绑定谷歌验证器，通过验证码成功登陆过一次，即算成功
        /// </summary>
        [SugarColumn(ColumnDescription = "是否绑定谷歌验证器")]
        public bool IsBindGoogle { get; set; }
        /// <summary>
        /// 谷歌验证码密钥
        /// </summary>
        [SugarColumn(ColumnDescription = "谷歌验证码密钥")]
        public string GoogleSerectKey { get; set; }
        /// <summary>
        /// 谷歌验证器app输入的密钥
        /// </summary>
        [SugarColumn(ColumnDescription = "谷歌验证器app输入的密钥")]
        public string GoogleMobileKey { get; set; }
        /// <summary>
        /// 谷歌验证绑定二维码
        /// </summary>
        [SugarColumn(ColumnDescription = "谷歌验证绑定二维码")]
        public string GoogleQrCode { get; set; }
    }
}



