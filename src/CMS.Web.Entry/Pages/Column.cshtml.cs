using CMS.Web.Entry.ViewModels;
using Furion;
using Furion.DataEncryption.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public class ColumnModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ColumnName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }

        public Site Site { get; set; }
        /// <summary>
        /// ����ģ��
        /// </summary>
        public string EngineResult { get; set; }

        private readonly IMemoryCache _cache;

        private readonly IRepository _repository;
        public ColumnModel(IMemoryCache cache, IRepository repository)
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
            if (ColumnName.IsEmpty() || Id.IsEmpty())
            {
                Response.Redirect("/notfound");
                return;
            }
            var columnId = Id.ToUpper().ToDESCDecrypt(App.Configuration["SecretKey"]).ToInt();
            if (columnId == 0)
            {
                Response.Redirect("/notfound");
                return;
            }

            if (PageIndex == 0) PageIndex = 1;
            await BuildHelper.AddVisitLog(site.Id, HttpContext, _repository);
            var dto = await _cache.GetOrCreateAsync($"{PagesConstants.SiteColumnPrefix}{site.Id}{columnId}{PageIndex}", async entry =>
            {

                var dto = new ColumnDto
                {
                    Site = site,
                };

                //վ����Ŀ
                (var columnList, var columnIds) = await BuildHelper.GetSiteColumns(_repository, site.Id);
                dto.ColumnList = columnList;
                dto.Column = await _repository.Queryable<Column>().FirstAsync(r=>r.Id==columnId);


                //��ȡ��������
                var childrenList = await _repository.Queryable<Column>().ToChildListAsync(it => it.ParentId, columnId);
                var childrenIds = childrenList.Select(r => r.Id).ToList();
                childrenIds.Add(columnId);

                //��ȡ��Ŀ����������
                Expression<Func<NewsOutputDto, bool>> predicate = m => childrenIds.Contains(m.ColumnId);
                (var hotList, var total) = await BuildHelper.GetNewsList(_repository, 1, 10, predicate);
                dto.HotList = hotList;

                //��ҳ��ȡ(��ȡ��ǰ��Ŀ���Ӽ�����)
                Expression<Func<NewsOutputDto, bool>> predicate1 = m => childrenIds.Contains(m.ColumnId);
                (var newsList, var total1) = await BuildHelper.GetNewsList(_repository, PageIndex, 20, predicate1);
                dto.NewsList = newsList;
                dto.PageIndex = PageIndex;
                dto.Total = total1;
                dto.TotalPage = BuildHelper.ComputerTotalPage(dto.Total);
                dto.PageList = BuildHelper.ComputerPage(dto.Column.Link, PageIndex, dto.TotalPage);

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return dto;
            });
            dto.IsLogin = HttpContext.Request.Cookies[PagesConstants.IsLogin].ToInt();
            dto.UserName = HttpContext.Request.Cookies[PagesConstants.UserName] ?? "".ToString();
            EngineResult = await BuildHelper.ViewEngine(site.TemplateName, "column.html", dto);
        }


    }
}

