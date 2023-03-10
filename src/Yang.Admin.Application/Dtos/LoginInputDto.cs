using System.ComponentModel.DataAnnotations;
namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 登录请求Dto
    /// </summary>
    public class LoginInputDto
    {
        /// <summary>
        /// 账户名
        /// </summary>
        [Required(ErrorMessage = "账号必填"), MinLength(3, ErrorMessage = "字符串长度不能少于3位")]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 验证码Id
        /// </summary>
        public string VerifyId { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string VerifyCode { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }
        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; }
    }
}



