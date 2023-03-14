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
    }
}



