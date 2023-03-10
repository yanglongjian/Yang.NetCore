using System.ComponentModel.DataAnnotations;
namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class SetPasswordInputDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "密码必须填写")]
        [MinLength(6, ErrorMessage = "密码不能小于6位")]
        public string NewPassword { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "密码必须填写")]
        [MinLength(6, ErrorMessage = "密码不能小于6位")]
        public string ConfirmNewPassword { get; set; }
    }


}


