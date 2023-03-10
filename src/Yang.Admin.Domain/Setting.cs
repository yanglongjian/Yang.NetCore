using SqlSugar;
using System;
using Yang.Core;


namespace Yang.Admin.Domain
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [SugarTable("Sys_Setting", "系统配置")]
    public class Setting : IEntity<int>, ICreatedTime, IUpdated<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        [SugarColumn(ColumnDescription = "配置信息")]
        public string SettingInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [SugarColumn(ColumnDescription = "更新人")]
        public int? UpdatedId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnDescription = "更新时间")]
        public DateTime? UpdatedTime { get; set; }
    }


    /// <summary>
    /// 系统配置项
    /// </summary>
    public class SettingInfo
    {
        /// <summary>
        /// 谷歌校验器
        /// </summary>
        public bool GoogleValidator { get; set; }
        /// <summary>
        /// 代理
        /// </summary>
        public bool Proxy { get; set; }
    }
}


