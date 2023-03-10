using CMS.Web.Entry.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;
using Yang.Core;
namespace CMS.Web.Entry.Pages
{
    [AllowAnonymous]
    public class SiteModel : PageModel
    {
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IRepository _repository;
        public SiteModel(IMemoryCache cache, IRepository repository)
        {

            _cache = cache;
            _repository = repository;
        }

        public async Task OnGet()
        {
            (var IsRedirect, var site) = await BuildHelper.GetSite(HttpContext,_repository);
            if (IsRedirect)
            {
                Response.Redirect(site.WebSite,true);
                return;
            }
            int siteId = site.Id;
            await _repository.Db.Updateable<Site>().SetColumns(r => r.Visit == r.Visit + 1).Where(r => r.Id == siteId).ExecuteCommandAsync();
            await BuildHelper.AddVisitLog(siteId, HttpContext, _repository);
  
            var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteIndexPrefix}{siteId}", async entry =>
            {
                var dto = new SiteDto
                {
                    Site = site,
                    ColumnNewsList = new List<ColumnNews>()
                };


                //վ����Ŀ
                (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, siteId);
                dto.ColumnList = columnList;


                //��ȡ��������
                Expression<Func<NewsOutputDto, bool>> predicate = m => columnIds.Contains(m.ColumnId);
                (var hotList, var total) = await BuildHelper.GetNewsList(_repository, 1, 10, predicate);
                dto.HotList = hotList;


                foreach (var item in dto.ColumnList)
                {
                    var childrenList = await _repository.Queryable<Column>().ToChildListAsync(it => it.ParentId, item.Id);
                    var childrenIds = childrenList.Select(r => r.Id).ToList();
                    childrenIds.Add(item.Id);

                    Expression<Func<NewsOutputDto, bool>> predicate1 = m => childrenIds.Contains(m.ColumnId);
                    (var newsList, var total1) = await BuildHelper.GetNewsList(_repository, 1, 6, predicate1);
                    dto.ColumnNewsList.Add(new ColumnNews
                    {
                        Id = item.Id,
                        ColumnName = item.Name,
                        Link = item.Link,
                        NewsList = newsList
                    });
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return dto;
            });

            dto.IsLogin = HttpContext.Request.Cookies[PagesConstants.IsLogin].ToInt();
            dto.UserName = HttpContext.Request.Cookies[PagesConstants.UserName] ?? "".ToString();
            EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "index.html", dto);

        }
    }
}

