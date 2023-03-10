using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CMS.Web.Entry.ViewModels;
using Furion;
using Furion.DataEncryption.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry.Pages
{
    [AllowAnonymous]
    public class NewsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IRepository _repository;
        public NewsModel(IMemoryCache cache, IRepository repository)
        {

            _cache = cache;
            _repository = repository;
        }

        public async Task OnGet()
        {
            (var IsRedirect, var site) = await BuildHelper.GetSite(HttpContext, _repository);
            if (IsRedirect)
            {
                Response.Redirect(site.WebSite, true);
                return;
            }
            await BuildHelper.AddVisitLog(site.Id, HttpContext, _repository);
            var newsId = Id.ToUpper().ToDESCDecrypt(App.Configuration["SecretKey"]).ToInt();
            var news = await _repository.Queryable<News>().FirstAsync(r => r.Id == newsId);
            if (news.IsNull())
            {
                Response.Redirect("/notfound");
                return;
            }
            await _repository.Db.Updateable<News>().SetColumns(r => r.Read == r.Read + 1).Where(r => r.Id == newsId).ExecuteCommandAsync();

            var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteNewsPrefix}{newsId}", async entry =>
            {
                var dto = new NewsDto
                {
                    Site = site,
                    News = news
                };
                //վ����Ŀ
                (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, site.Id);
                dto.ColumnList = columnList;


                //��ȡ��������
                Expression<Func<NewsOutputDto, bool>> predicate = m => columnIds.Contains(m.ColumnId);
                (var hotList, var total) = await BuildHelper.GetNewsList(_repository, 1, 10, predicate);
                dto.HotList = hotList;

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return dto;
            });
            dto.IsLogin = HttpContext.Request.Cookies[PagesConstants.IsLogin].ToInt();
            dto.UserName = HttpContext.Request.Cookies[PagesConstants.UserName]??"".ToString();
            EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "news.html", dto);
        }

    }
}

