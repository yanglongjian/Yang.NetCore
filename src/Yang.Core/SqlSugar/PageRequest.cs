using SqlSugar;

namespace Yang.Core
{
    /// <summary>
    /// 基础分页信息
    /// </summary>
    public class BasePageRequest
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页数大小
        /// </summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 分页请求
    /// </summary>
    public class PageRequest : BasePageRequest
    {

        /// <summary>
        /// 条件
        /// </summary>
        public FilterRule[] FilterRules { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public SortCondition[] SortConditions { get; set; }
    }

    #region 条件

    /// <summary>
    /// 筛选规则
    /// </summary>
    public class FilterRule
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///  操作类型
        /// </summary>
        public FilterOperate Operate { get; set; }
    }

    /// <summary>
    /// 筛选操作方式
    /// </summary>
    public enum FilterOperate
    {
        /// <summary>
        /// 并且
        /// </summary>
        And = 1,

        /// <summary>
        /// 或者
        /// </summary>
        Or = 2,

        /// <summary>
        /// 等于
        /// </summary>
        Equal = 3,

        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual = 4,

        /// <summary>
        /// 小于
        /// </summary>
        Less = 5,

        /// <summary>
        /// 小于或等于
        /// </summary>
        LessOrEqual = 6,

        /// <summary>
        /// 大于
        /// </summary>
        Greater = 7,

        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterOrEqual = 8,

        /// <summary>
        /// 以……开始
        /// </summary>
        StartsWith = 9,

        /// <summary>
        /// 以……结束
        /// </summary>
        EndsWith = 10,

        /// <summary>
        /// 字符串的包含（相似）
        /// </summary>
        Contains = 11,

        /// <summary>
        /// 字符串的不包含
        /// </summary>
        NotContains = 12,
    }

    #endregion

    #region 排序
    /// <summary>
    /// 排序条件
    /// </summary>
    public class SortCondition
    {
        public string SortField { get; set; }
        public OrderByType OrderByType { get; set; }
    }
    #endregion
}



