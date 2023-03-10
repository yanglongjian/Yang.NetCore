using CMS.Web.Entry.ViewModels;
using Furion.LinqBuilder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry.Pages
{
    [AllowAnonymous]
    public class SearchModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchKey { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IRepository _repository;
        public SearchModel(IMemoryCache cache, IRepository repository)
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

            if (PageIndex == 0) PageIndex = 1;
            await BuildHelper.AddVisitLog(site.Id, HttpContext, _repository);
            var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteSearchPrefix}{site.Id}{SearchKey}{PageIndex}", async entry =>
            {
                var dto = new SearchDto
                {
                    SearchKey = SearchKey,
                    Site = site,
                    NewsList = new List<NewsOutputDto>()
                };

                //վ����Ŀ
                (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, site.Id);
                dto.ColumnList = columnList;


                //��ȡ��������
                Expression<Func<NewsOutputDto, bool>> predicate = m => columnIds.Contains(m.ColumnId);
                (var hotList, var total) = await BuildHelper.GetNewsList(_repository, 1, 10, predicate);
                dto.HotList = hotList;

                //��ҳ��ȡ
                if (!string.IsNullOrWhiteSpace(SearchKey))
                {
                    Expression<Func<NewsOutputDto, bool>> predicate1 = m => columnIds.Contains(m.ColumnId);
                    predicate1 = predicate1.And(r => r.Title.Contains(SearchKey));
                   
                    (var newsList, var total1) = await BuildHelper.GetNewsList(_repository, PageIndex, 20, predicate1);
                    dto.NewsList = newsList;
                    dto.PageIndex = PageIndex;
                    dto.Total = total1;
                    dto.TotalPage = BuildHelper.ComputerTotalPage(dto.Total);
                    dto.PageList = BuildHelper.ComputerPage($"/search/{SearchKey}", PageIndex, dto.TotalPage);
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return dto;

            });

            dto.IsLogin = HttpContext.Request.Cookies[PagesConstants.IsLogin].ToInt();
            dto.UserName = HttpContext.Request.Cookies[PagesConstants.UserName] ?? "".ToString();

            EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "search.html", dto);
        }
    }
}

