using Google.Authenticator;

namespace Yang.Core
{
    /// <summary>
    /// 谷歌验证器扩展
    /// </summary>
    public class GoogleExtensions
    {
        /// <summary>
        /// 创建安装用户谷歌验证器(第一次需要更新到用户信息中)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accountTile"></param>
        /// <param name="googleSerectKey"></param>
        /// <returns>(二维码,App输入密钥)</returns>
        public static (string, string) GenerateSetupCode(string userName, string accountTile, string googleSerectKey)
        {
            TwoFactorAuthenticator tfa = new();
            var setupInfo = tfa.GenerateSetupCode(userName, accountTile, googleSerectKey, false);
            return (setupInfo.QrCodeSetupImageUrl, setupInfo.ManualEntryKey);
        }


        /// <summary>
        /// 校验谷歌验证码
        /// </summary>
        /// <param name="googleSerectKey"></param>
        /// <param name="verifyCode"></param>
        /// <returns></returns>
        public static bool ValidateTwoFactorPIN(string googleSerectKey, string verifyCode)
        {
            TwoFactorAuthenticator tfa = new();
            return tfa.ValidateTwoFactorPIN(googleSerectKey, verifyCode);
        }
    }
}



