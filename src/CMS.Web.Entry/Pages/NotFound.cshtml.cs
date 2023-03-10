using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Web.Entry.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry.Pages
{
    [AllowAnonymous]
    public class NotFoundModel : PageModel
    {
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IRepository _repository;
        public NotFoundModel(IMemoryCache cache, IRepository repository)
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
            if (site != null)
            {
                var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteNotFoundPrefix}{site.Id}", async entry =>
                {
                    var dto = new NotFoundDto
                    {
                        Site = site,
                    };
                    //վ����Ŀ
                    (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, site.Id);
                    dto.ColumnList = columnList;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return dto;
                });
                EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "404.html", dto);
            }
            else
            {
                EngineResult = await BuildHelper._404ViewEngine("site", "404.html");
            }
        }
    }
}

