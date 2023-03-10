using SqlSugar;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 谷歌验证信息
    /// </summary>
    [SugarTable("Sys_UserGoogle", "谷歌验证信息")]
    public class UserGoogle : IEntity<int>
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
        /// 是否绑定过谷歌验证器，通过验证码成功登陆过一次，即算成功
        /// </summary>

        [SugarColumn(ColumnDescription = "是否绑定")]
        public bool IsBind { get; set; }

        /// <summary>
        /// 谷歌验证码密钥
        /// </summary>
        [SugarColumn(ColumnDescription = "谷歌验证码密钥")]
        public string SerectKey { get; set; }
        /// <summary>
        /// 谷歌验证器app输入的密钥
        /// </summary>
        [SugarColumn(ColumnDescription = "谷歌验证器app输入的密钥")]
        public string MobileKey { get; set; }

        /// <summary>
        /// 谷歌验证绑定二维码
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "二维码")]
        public string QrCode { get; set; }
    }
}

