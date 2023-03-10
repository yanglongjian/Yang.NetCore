using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Core
{
    /// <summary>
    /// 通用返回结果
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public AjaxResult(AjaxResultType code, object data = null, string message = "")
        {
            Code = code;
            Message = message.IsEmpty() ? "获取成功" : message;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="i"></param>
        public AjaxResult(AjaxResultType code, int i)
        {
            Code = code;
            Message = $"{i}个信息操作成功";
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public AjaxResult(AjaxResultType code, string message)
        {
            Code = code;
            Message = message;
        }


        /// <summary>
        /// 状态码
        /// </summary>
        public AjaxResultType Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }


    /// <summary>
    /// 操作结果类型的枚举
    /// </summary>
    public enum AjaxResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 操作没引发任何变化
        /// </summary>
        NoChanged = 201,

        /// <summary>
        /// 警告消息
        /// </summary>
        Warning = 203,

        /// <summary>
        /// 用户未登录
        /// </summary>
        UnAuth = 401,

        /// <summary>
        /// 已登录，但权限不足
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 资源未找到
        /// </summary>
        NoFound = 404,

        /// <summary>
        /// 错误异常
        /// </summary>
        Error = 500,
    }
}


