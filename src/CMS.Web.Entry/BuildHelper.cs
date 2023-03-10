using CMS.Web.Entry.ViewModels;
using Furion;
using Furion.LinqBuilder;
using Furion.ViewEngine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yang.Cms.Application.Dtos;
using Yang.Cms.Domain;
using Yang.Core;

namespace CMS.Web.Entry
{
    public class BuildHelper
    {
        /// <summary>
        /// 添加访问日志
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="context"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task AddVisitLog(int siteId, Microsoft.AspNetCore.Http.HttpContext context, IRepository repository)
        {
            var userAgent = context.Request.Headers.ToArray().FirstOrDefault(r => r.Key == "User-Agent").Value.ToString();
            var referer = context.Request.Headers.ToArray().FirstOrDefault(r => r.Key == "Referer").Value.ToString();
            var ip = context.GetClientIp();
            var url = context.Request.Path.ToString();
            await repository.Insert(new VisitLog
            {
                SiteId = siteId,
                Url = url,
                Ip = ip,
                UserAgent = userAgent,
                Referer = referer,
                Keyword = ""
            });
        }

        /// <summary>
        /// 视图引擎
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="htmlName"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static async Task<string> ViewEngine(string templateName, string htmlName, object dto)
        {
            string result = string.Empty;
            var _env = App.GetService<IWebHostEnvironment>();
            var _viewEngine = App.GetService<IViewEngine>();
            string dir = Path.Combine(_env.WebRootPath, App.Configuration["TemplateFolder"]);
            string templateDir = Path.Combine(dir, templateName);
            string viewPath = Path.Combine(templateDir, htmlName);
            byte[] buffer = File.ReadAllBytes(viewPath);
            string template = System.Text.Encoding.UTF8.GetString(buffer);
            try
            {
                result = await _viewEngine.RunCompileFromCachedAsync(
                       content: template,
                       model: dto,
                       builderAction: builder =>
                       {
                           builder.AddAssemblyReferenceByName("Yang.Cms.Domain");
                           builder.AddAssemblyReferenceByName("System.Collections");
                           builder.AddAssemblyReferenceByName("System");
                           builder.AddAssemblyReferenceByName("System.Net");
                           builder.AddAssemblyReferenceByName("System.Linq");
                       });
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 站点404渲染
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="htmlName"></param>
        /// <returns></returns>
        public static async Task<string> _404ViewEngine(string dirName, string htmlName)
        {
            string result = string.Empty;
            var _env = App.GetService<IWebHostEnvironment>();
            var _viewEngine = App.GetService<IViewEngine>();
            string dir = Path.Combine(_env.WebRootPath, dirName);
            string viewPath = Path.Combine(dir, htmlName);
            byte[] buffer = File.ReadAllBytes(viewPath);
            string template = System.Text.Encoding.UTF8.GetString(buffer);
            try
            {
                result = await _viewEngine.RunCompileFromCachedAsync(content: template);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取站点信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static async Task<(bool IsRedirect, Site site)> GetSite(Microsoft.AspNetCore.Http.HttpContext context, IRepository repository)
        {
            var host = context.Request.Headers.ToArray().FirstOrDefault(r => r.Key == "Host").Value.ToString();
            string webSite = host[(host.IndexOf('.') + 1)..];
            var site = await repository.Queryable<Site>().Where(r => r.WebSite.Contains(host)).FirstAsync();
            if (site == null)
            {
                site = await repository.Queryable<Site>().Where(r => r.WebSite.Contains(webSite)).FirstAsync();
            }

            if (site == null)
            {

                site = await repository.Queryable<Site>().OrderBy(r => r.Id).FirstAsync();
                return (true, site);
            }

            return (false, site);

        }

        /// <summary>
        /// 获取站点栏目
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static async Task<(List<Column> columnList, List<int> columnIds)> GetSiteColumns(IRepository repository, int siteId)
        {
            //站点所有栏目编号
            var columnIds = await repository.Queryable<SiteColumn>().Where(r => r.SiteId == siteId).Select(r => r.ColumnId).ToListAsync();

            var list = await repository.Queryable<Column>().Where(r => columnIds.Contains(r.Id)).OrderBy(r => r.OrderCode)
                .ToTreeAsync(it => it.Children, it => it.ParentId, 0);

            var columnList = list ?? new List<Column>();
            return (columnList, columnIds);
        }



        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static async Task<(List<NewsOutputDto> rows, int total)> GetNewsList(IRepository repository,
            int pageIndex, int pageSize, Expression<Func<NewsOutputDto, bool>> expression)
        {
            //获取发布的新闻
            expression = expression.And(r => r.Status == 0);
            RefAsync<int> total = 0;
            var newsList = await repository.Queryable<News>()
                 .Select(r => new NewsOutputDto { Id = r.Id.SelectAll() })
                 .Where(expression)
                 .OrderBy(it => new
                 {
                     isTop = SqlFunc.Desc(it.IsTop),
                     read = SqlFunc.Desc(it.Read),
                     createdTime = SqlFunc.Desc(it.CreatedTime),
                 })
                 .ToPageListAsync(pageIndex, pageSize, total);

            return (newsList ?? new List<NewsOutputDto>(), total.Value);
        }


        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="total"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int ComputerTotalPage(int total, int pageSize = 20)
        {
            int pageCount = total / pageSize;
            if (total % pageSize != 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        /// <summary>
        /// 计算分页
        /// </summary>
        /// <param name="link"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPage"></param>
        /// <returns></returns>
        public static List<PageDto> ComputerPage(string link, int currentPage, int totalPage)
        {
            List<PageDto> list = new();
            const int maxPagesToShow = 5;
            int startPage, endPage;

            if (totalPage <= maxPagesToShow)
            {
                // 如果总页数小于等于要显示的分页数，则显示所有分页
                startPage = 1;
                endPage = totalPage;
            }
            else
            {
                // 如果总页数大于要显示的分页数，则计算起始页码和结束页码
                var pagesBeforeAndAfter = (maxPagesToShow - 1) / 2;
                if (currentPage <= pagesBeforeAndAfter)
                {
                    // 当前页码靠近起始页
                    startPage = 1;
                    endPage = maxPagesToShow;
                }
                else if (currentPage + pagesBeforeAndAfter >= totalPage)
                {
                    // 当前页码靠近末尾页
                    startPage = totalPage - maxPagesToShow + 1;
                    endPage = totalPage;
                }
                else
                {
                    // 当前页码在中间
                    startPage = currentPage - pagesBeforeAndAfter;
                    endPage = currentPage + pagesBeforeAndAfter;
                }
            }

            if (startPage > 1)
            {
                list.Add(new PageDto
                {
                    Page = "<",
                    Link = $"{link}/{startPage - 1}",
                });
            }


            for (var i = startPage; i <= endPage; i++)
            {
                list.Add(new PageDto
                {
                    Page = i.ToString(),
                    Link = $"{link}/{i}",
                });
            }

            if (endPage < totalPage)
            {
                list.Add(new PageDto
                {
                    Page = ">",
                    Link = $"{link}/{currentPage + 1}",
                });
            }

            return list;
        }
    }
}

