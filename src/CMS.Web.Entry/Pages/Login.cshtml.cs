using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Web.Entry.ViewModels;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IRepository _repository;
        public LoginModel(IMemoryCache cache, IRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }
        public async Task OnGet()
        {
            Response.Cookies.Delete(PagesConstants.IsLogin);
            Response.Cookies.Delete(PagesConstants.UserId);
            Response.Cookies.Delete(PagesConstants.UserName);
            (var IsRedirect, var site) = await BuildHelper.GetSite(HttpContext, _repository);
            if (IsRedirect)
            {
                Response.Redirect(site.WebSite, true);
                return;
            }
            await BuildHelper.AddVisitLog(site.Id, HttpContext, _repository);
            var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteLoginhPrefix}{site.Id}", async entry =>
            {
                var dto = new NotFoundDto
                {
                    Site = site,
                };
                (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, site.Id);
                dto.ColumnList = columnList;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return dto;
            });
            EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "login.html", dto);
        }


        /// <summary>
        /// ��¼
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostLoginIn(LoginDto dto)
        {
            var account = await _repository.Queryable<Member>().FirstAsync(r => r.Account == dto.Account);
            if (account.IsNull())
                return new JsonResult(new
                {
                    Success = false,
                    Msg = "�˺Ų�����"
                });

            if (MD5Encryption.Encrypt(dto.Password) != account.Password)
                return new JsonResult(new
                {
                    Success = false,
                    Msg = "���벻��ȷ"
                });


            Response.Cookies.Append(PagesConstants.IsLogin, "1");
            Response.Cookies.Append(PagesConstants.UserId, account.Id.ToString());
            Response.Cookies.Append(PagesConstants.UserName, account.Account);

            return new JsonResult(new
            {
                Success = true,
                Msg = "��¼�ɹ�"
            });

        }


        /// <summary>
        /// ע��
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRegister(RegisterDto dto)
        {
            if (dto.Password != dto.ComfirmedPassword)
                return new JsonResult(new
                {
                    Success = false,
                    Msg = "�������벻һ��"
                });

            var account = await _repository.Queryable<Member>().FirstAsync(r => r.Account == dto.Account);
            if (account.IsNotNull())
                return new JsonResult(new
                {
                    Success = false,
                    Msg = "�˺��Ѵ���"
                });


            var member = new Member
            {
                Account = dto.Account,
                Password = MD5Encryption.Encrypt(dto.Password),
                NickName = dto.Account,
                Avatar = "",
                Mobile = "",
                CreatedTime = DateTime.Now
            };

            await _repository.Insert(member);

            return new JsonResult(new
            {
                Success = true,
                Msg = "ע��ɹ�"
            });
        }

    }


    /// <summary>
    /// ��¼��Ϣ
    /// </summary>
    public class LoginDto
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }


    /// <summary>
    /// ��¼��Ϣ
    /// </summary>
    public class RegisterDto
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string ComfirmedPassword { get; set; }
    }
}

