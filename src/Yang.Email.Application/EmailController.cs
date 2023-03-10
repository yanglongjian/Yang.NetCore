using Furion;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Yang.Core;
using Yang.Email.Application.Dtos;

namespace Yang.Email.Application
{
    /// <summary>
    /// 邮件发送管理
    /// </summary>
    [NonUnify]
    [ModuleInfo("邮件管理", "邮件管理", Module = "Email", OrderCode = 1)]
    public class EmailController : IDynamicApiController
    {
        /// <summary>
        /// 邮箱验证码键值前缀
        /// </summary>
        private const string EmailCodePrefix = "EmailCode_";

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<AjaxResult> SendEmailCode(EmailInputDto dto)
        {
            var options = App.GetConfig<EmailOptions>("EmailOptions");
            if (options.IsNull()) return new AjaxResult(AjaxResultType.Error, "邮箱发送方配置未设置");
            if (!dto.Email.IsEmail()) return new AjaxResult(AjaxResultType.Error, "邮箱地址不正确");
            var emailCode = RedisHelper.Get($"{EmailCodePrefix}{dto.Email}");
            if (emailCode.IsNotEmpty()) return new AjaxResult(AjaxResultType.Warning, "已获取邮箱验证码,无需重复获取");

            var code = ValidateCoder.GetCode(6, ValidateCodeType.Number);
            try
            {
                string subject = "注册邮箱验证码";
                string body =
                   @$"亲爱的用户 <strong>{dto.Email}</strong>，您好！<br>
                    欢迎注册，激活邮箱请填写验证码：<strong>{code}</strong><br>
                    祝您使用愉快！";

                await SendEmailAsync(options, dto.Email, subject, body);

                RedisHelper.Set($"{EmailCodePrefix}{dto.Email}", code, 180);
                return new AjaxResult(AjaxResultType.Success, "发送成功,请注意查收");
            }
            catch (Exception ex)
            {
                return new AjaxResult(AjaxResultType.Error, ex.Message);
            }

        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="options">发件人配置</param>
        /// <param name="email">接收人Email</param>
        /// <param name="subject">Email标题</param>
        /// <param name="body">Email内容</param>
        /// <returns></returns>
        private static async Task SendEmailAsync(EmailOptions options, string email, string subject, string body)
        {
            string host = options.Host,
                displayName = options.DisplayName,
                userName = options.UserName,
                password = options.Password;
            bool enableSsl = options.EnableSsl;
            int port = options.Port;
            if (port == 0)
            {
                port = enableSsl ? 465 : 25;
            }

            SmtpClient client = new(host, port)
            {
                UseDefaultCredentials = true,
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(userName, password)
            };

            string fromEmail = userName.Contains("@") ? userName : string.Format("{0}@{1}", userName, client.Host.Replace("smtp.", ""));
            MailMessage mail = new()
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(email);
            await client.SendMailAsync(mail);

        }
    }
}

