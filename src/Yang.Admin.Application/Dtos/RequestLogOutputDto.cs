using System;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 请求日志 输出DTO
    /// </summary>
    public class RequestLogOutputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 请求Api
        /// </summary>
        public string RequestPath { get; set; }
        /// <summary>
        /// 请求IP
        /// </summary>
        public string RequestIp { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public double Elapsed { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public int? CreatedId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
    }
}



