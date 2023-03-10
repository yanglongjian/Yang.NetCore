using System.ComponentModel.DataAnnotations;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class UserPasswordInputDto
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
        [DataValidation(AdminValidationTypes.StrongPassword)]
        public string NewPassword { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "密码必须填写")]
        [DataValidation(AdminValidationTypes.StrongPassword)]
        public string ConfirmNewPassword { get; set; }
    }


}



