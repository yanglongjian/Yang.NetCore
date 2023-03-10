using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yang.Core;

namespace Yang.Admin.Domain
{
    /// <summary>
    /// 请求日志
    /// </summary>
    [SugarTable("Sys_RequestLog", "请求日志")]
    public class RequestLog : IEntity<string>, ICreatedAudit<int>
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnDescription = "编号")]
        public string Id { get; set; }
        /// <summary>
        /// 请求Api
        /// </summary>
        [SugarColumn(ColumnDescription = "请求Api")]
        public string RequestPath { get; set; }
        /// <summary>
        /// 请求IP
        /// </summary>
        [SugarColumn(ColumnDescription = "请求IP")]
        public string RequestIp { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "参数")]
        public string Args { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        [SugarColumn(ColumnDescription = "耗时")]
        public double Elapsed { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        [SugarColumn(ColumnDescription = "状态码")]
        public AjaxResultType Code { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        [SugarColumn(ColumnDescription = "异常消息")]
        public string Message { get; set; }

        /// <summary>
        /// 堆栈跟踪
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "堆栈跟踪")]
        public string StackTrace { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者")]
        public int? CreatedId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public DateTime CreatedTime { get; set; }

    }
}



