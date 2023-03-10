using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Core;

namespace Yang.Gather.Domain
{
    /// <summary>
    /// 采集配置
    /// </summary>
    [SugarTable("App_CollectConfig", "采集配置")]
    public class CollectConfig : IEntity<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "编号")]
        public int Id { get; set; }

        /// <summary>
        /// 采集编号
        /// </summary>
        [SugarColumn(ColumnDescription = "采集编号")]
        public int CollectId { get; set; }

        /// <summary>
        /// 索引编号
        /// </summary>
        [SugarColumn(ColumnDescription = "索引编号")]
        public int Index { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        [SugarColumn(ColumnDescription = "字段")]
        public string Field { get; set; }

        /// <summary>
        /// 从详情页获取
        /// </summary>
        [SugarColumn(ColumnDescription = "从详情页获取")]
        public bool FromDetail { get; set; }

        /// <summary>
        /// 类型 0-字符匹配 1-键值匹配 2-正则匹配  3-xpath
        /// </summary>
        [SugarColumn(ColumnDescription = "类型")]
        public FieldType Type { get; set; }

        /// <summary>
        /// Json键名
        /// </summary>
        [SugarColumn(ColumnDescription = "Json键名")]
        public string JsonKey { get; set; }

        /// <summary>
        /// 统计
        /// </summary>
        [SugarColumn(DefaultValue = "0", ColumnDescription = "统计")]
        public bool Count { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        [SugarColumn(ColumnDescription = "索引")]
        public string SlotIndex { get; set; }

        /// <summary>
        /// html内容
        /// </summary>
        [SugarColumn(ColumnDescription = "html内容")]
        public bool InnerHtml { get; set; }


        /// <summary>
        /// 字符开始
        /// </summary>
        [SugarColumn(ColumnDescription = "字符开始")]
        public string CharacterBegin { get; set; }

        /// <summary>
        /// 字符结束
        /// </summary>
        [SugarColumn(ColumnDescription = "字符结束")]
        public string CharacterEnd { get; set; }

        /// <summary>
        /// 参数拼接
        /// </summary>
        [SugarColumn(ColumnDescription = "参数拼接")]
        public string StringFormat { get; set; }

        /// <summary>
        /// 替换词汇
        /// </summary>
        [SugarColumn(ColumnDescription = "替换词汇")]
        public bool IsDisplace { get; set; }

        /// <summary>
        /// 保存本地图片
        /// </summary>
        [SugarColumn(ColumnDescription = "保存本地图片")]
        public bool SaveImg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDescription = "备注")]
        public string Remark { get; set; }
    }

    /// <summary>
    /// 字段采集类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 正则
        /// </summary>
        选择器 = 0,
        /// <summary>
        /// json键值
        /// </summary>
        JSON = 1,
        /// <summary>
        /// 正则匹配
        /// </summary>
        正则 = 2,
        /// <summary>
        /// json键值
        /// </summary>
        XPATH = 3
    }
}

