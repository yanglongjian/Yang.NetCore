using Furion.DataValidation;
using System.Text.RegularExpressions;

namespace Yang.Admin.Application.Dtos
{
    /// <summary>
    /// 框架自定义校验
    /// </summary>
    [ValidationType]
    public enum AdminValidationTypes
    {
        /// <summary>
        /// 强密码类型
        /// </summary>
        [ValidationItemMetadata(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$", "必须须包含大小写字母和数字的组合，不能使用特殊字符，长度在8-16之间")]
        StrongPassword,
        /// <summary>
        /// 以 Furion 字符串开头，忽略大小写
        /// </summary>
        [ValidationItemMetadata(@"^(furion).*", "默认提示：必须以Fur字符串开头，忽略大小写", RegexOptions.IgnoreCase)]
        StartWithFurString
    }
}



