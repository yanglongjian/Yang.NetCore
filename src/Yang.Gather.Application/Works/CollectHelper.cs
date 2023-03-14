//using AngleSharp.Html.Parser;
//using Flurl.Http;
//using Furion;
//using Furion.ClayObject;
//using HtmlAgilityPack;
//using Microsoft.AspNetCore.Hosting;
//using NPinyin;
//using PuppeteerSharp;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Yang.Core;
//using Yang.Gather.Domain;

//namespace Yang.Gather.Application.Works
//{
//    /// <summary>
//    /// 采集帮助类
//    /// </summary>
//    public class CollectHelper
//    {
//        private static readonly IWebHostEnvironment _env = App.GetService<IWebHostEnvironment>();
//        private static readonly string dir = Path.Combine(_env.WebRootPath, "upload-files");

//        /// <summary>
//        /// 执行
//        /// </summary>
//        /// <returns></returns>
//        public static async Task<int> Run(IRepository repository, Collect collect, bool isTask = false)
//        {
//            var displaceList = await repository.Queryable<Displace>().ToListAsync();
//            (var total, var obj) = await HandleGather(repository, displaceList, collect);
//            if (isTask)
//            {
//                collect.NextTime = DateTime.Now.AddMinutes(collect.Interval);
//                await repository.Update(collect, "NextTime,Parameter");
//            }
//            else
//            {
//                await repository.Update(collect, "Parameter");
//            }
//            return total;
//        }


//        /// <summary>
//        /// 采集
//        /// </summary>
//        /// <returns></returns>
//        public static async Task<(int num, dynamic clay)> HandleGather(IRepository repository, List<Displace> displaceList, Collect collect, bool isPreview = false)
//        {
//            #region 初始化无头浏览器
//            IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions
//            {
//                Headless = true
//            });
//            IPage page = await browser.NewPageAsync();
//            #endregion

//            int collectNum = 0;
//            dynamic clay = Clay.Object(new { });
//            //按页数采集,为0默认采集一页
//            if (collect.Count == 0) collect.Count = 1;
//            for (var j = 0; j < collect.Count; j++)
//            {

//                #region 获取内容
//                var url = collect.Parameter.IsEmpty() ? collect.Url : string.Format(collect.Url, collect.Parameter);
//                string htmlString = string.Empty;
//                switch (collect.Type)
//                {
//                    case 2:
//                        await page.GoToAsync(url);
//                        htmlString = await page.GetContentAsync();
//                        break;
//                    default:
//                        htmlString = await url.GetStringAsync();
//                        break;
//                }

//                if (collect.Begin.IsNotEmpty() && collect.End.IsNotEmpty())
//                {
//                    string pattern = $@"{Regex.Escape(collect.Begin)}(.*?)\s*{Regex.Escape(collect.End)}";
//                    Regex regex = new(pattern, RegexOptions.Singleline);
//                    Match match = regex.Match(htmlString);
//                    htmlString = match.Success ? match.Groups[1].Value.Trim() : string.Empty;
//                }
//                if (htmlString.IsEmpty()) break;
//                #endregion

//                (var num, var obj) = await GatherByPage(page, repository, htmlString, displaceList, collect, isPreview);
//                collectNum += num;
//                clay = obj;
//                if (isPreview)
//                {
//                    break;
//                }
//                GetNextRequest(displaceList, collect, htmlString);
//            }
//            await page.CloseAsync();
//            await browser.CloseAsync();
//            return (collectNum, clay);
//        }


//        /// <summary>
//        /// 按页采集
//        /// </summary>
//        /// <param name="page"></param>
//        /// <param name="repository"></param>
//        /// <param name="htmlString"></param>
//        /// <param name="displaceList"></param>
//        /// <param name="collect"></param>
//        /// <param name="isPreview"></param>
//        /// <returns></returns>
//        public static async Task<(int num, dynamic clay)> GatherByPage(IPage page, IRepository repository, string htmlString,
//            List<Displace> displaceList, Collect collect, bool isPreview = false)
//        {
//            int num = 0;
//            var newsType = typeof(News);
//            var columnType = typeof(Column);
//            dynamic clay = Clay.Object(new { });
//            try
//            {
//                //总记录数
//                int total = GetFieldValue(displaceList, collect, htmlString, "Total").ToInt();
//                if (total == 0) return (num, clay);

//                for (var i = 0; i < total; i++)
//                {
//                    //新闻编号--唯一值判断重复
//                    var newsId = GetFieldValue(displaceList, collect, htmlString, "NewsId", i);
//                    if (await IsExist(repository, collect, newsId, isPreview))
//                    {
//                        Console.Write($"{i + 1}--");
//                        continue;
//                    }
//                    //详情页
//                    var detailUrl = GetFieldValue(displaceList, collect, htmlString, "From", i);
//                    var detailString = await GetDetailHtml(page, collect, detailUrl);

//                    #region 字段赋值
//                    var entity = new News
//                    {
//                        ColumnId = collect.ColumnId,
//                        Status = collect.IsPublish ? 0 : 1,
//                        From = detailUrl,
//                        NewsId = newsId,
//                    };
//                    var column = new Column
//                    {
//                        NewsId = newsId,
//                    };
//                    clay["Total"] = total;
//                    clay["NewsId"] = newsId;
//                    clay["From"] = detailUrl;
//                    #endregion

//                    #region 循环字段配置
//                    bool isSuccess = true;
//                    string[] fieldList = new string[4] { "Total", "Extra", "From", "NewsId" };
//                    var configList = collect.ConfigList.Where(r => !fieldList.Contains(r.Field));

//                    foreach (var item in configList)
//                    {
//                        try
//                        {

//                            string value = item.FromDetail ?
//                                    GetFieldValue(displaceList, collect, detailString, item.Field, 0)
//                                    : GetFieldValue(displaceList, collect, htmlString, item.Field, i);

//                            clay[item.Field] = value;


//                            if (collect.CollectType == 0)
//                            {
//                                var propertyInfo = newsType.GetProperty(item.Field);
//                                if (propertyInfo.PropertyType == typeof(int))
//                                {
//                                    newsType.GetProperty(item.Field).SetValue(entity, value.ToInt());
//                                }
//                                else if (propertyInfo.PropertyType == typeof(DateTime))
//                                {
//                                    newsType.GetProperty(item.Field).SetValue(entity, value.ToDateTime());
//                                }
//                                else
//                                {
//                                    newsType.GetProperty(item.Field).SetValue(entity, value);
//                                }
//                            }
//                            else
//                            {

//                                if (item.Field == "Title")
//                                {
//                                    columnType.GetProperty(item.Field).SetValue(column, value);
//                                    columnType.GetProperty("Name").SetValue(column, value);
//                                    string pinyin = ConvertToPinYin(value);
//                                    columnType.GetProperty("Pinyin").SetValue(column, pinyin);
//                                }
//                                else
//                                {
//                                    var propertyInfo = columnType.GetProperty(item.Field);
//                                    if (propertyInfo.PropertyType == typeof(int))
//                                    {
//                                        columnType.GetProperty(item.Field).SetValue(column, value.ToInt());
//                                    }
//                                    else if (propertyInfo.PropertyType == typeof(DateTime))
//                                    {
//                                        columnType.GetProperty(item.Field).SetValue(column, value.ToDateTime());
//                                    }
//                                    else
//                                    {
//                                        columnType.GetProperty(item.Field).SetValue(column, value);
//                                    }
//                                }

//                            }
//                        }
//                        catch
//                        {
//                            clay[item.Field] = string.Empty;
//                            isSuccess = false;
//                        }
//                    }
//                    #endregion

//                    //如果是预览直接跳出循环
//                    if (isPreview)
//                    {
//                        break;
//                    }

//                    #region 采集正常才插入
//                    if (isSuccess && !isPreview)
//                    {
//                        try
//                        {
//                            if (collect.CollectType == 0)
//                            {
//                                await repository.Insert(entity);
//                            }
//                            else
//                            {
//                                await repository.Insert(column);
//                            }
//                            Console.Write($"{i + 1}++");
//                            num++;
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine(ex.Message);
//                        }
//                    }
//                    #endregion
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return (num, clay);
//        }
//        /// <summary>
//        /// 中文转拼音
//        /// </summary>
//        /// <param name="chineseText"></param>
//        /// <returns></returns>
//        private static string ConvertToPinYin(string chineseText)
//        {
//            var pinyinWithoutTone = Pinyin.GetPinyin(chineseText);
//            // 去除空格并转换为小写
//            pinyinWithoutTone = pinyinWithoutTone.Replace(" ", "").ToLower();
//            return pinyinWithoutTone;
//        }


//        /// <summary>
//        /// 判断是否存在
//        /// </summary>
//        /// <param name="repository"></param>
//        /// <param name="collect"></param>
//        /// <param name="newsId"></param>
//        /// <param name="isPreview"></param>
//        /// <returns></returns>
//        private static async Task<bool> IsExist(IRepository repository, Collect collect, string newsId, bool isPreview = false)
//        {
//            if (isPreview) return false;

//            if (collect.CollectType == 0)
//            {
//                return await repository.IsExist<News>(r => r.NewsId == newsId);
//            }
//            else
//            {
//                return await repository.IsExist<Column>(r => r.NewsId == newsId);
//            }
//        }

//        /// <summary>
//        /// 获取详情页内容
//        /// </summary>
//        /// <param name="page"></param>
//        /// <param name="collect"></param>
//        /// <param name="url"></param>
//        /// <returns></returns>
//        private static async Task<string> GetDetailHtml(IPage page, Collect collect, string url)
//        {
//            string detailString = string.Empty;
//            if (collect.ConfigList.Any(r => r.FromDetail))
//            {
//                await page.GoToAsync(url);
//                detailString = await page.GetContentAsync();
//            }
//            return detailString;
//        }



//        /// <summary>
//        /// 获取下一次请求参数
//        /// </summary>
//        /// <param name="collect"></param>
//        /// <param name="htmlString"></param>
//        /// <returns></returns>
//        private static void GetNextRequest(List<Displace> displaceList, Collect collect, string htmlString)
//        {
//            //叠加优先级更高
//            if (collect.IsAdd)
//            {
//                collect.Parameter = (collect.Parameter.ToInt() + 1).ToString();
//            }
//            else
//            {
//                string extra = GetFieldValue(displaceList, collect, htmlString, "Extra");
//                if (extra.IsNotEmpty())
//                {
//                    collect.Parameter = extra;
//                }
//            }
//        }



//        #region  字段内容赋值

//        /// <summary>
//        /// 获取字段值
//        /// </summary>
//        /// <param name="collect"></param>
//        /// <param name="htmlString"></param>
//        /// <param name="field"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        private static string GetFieldValue(List<Displace> displaceList, Collect collect, string htmlString, string field, int index = 0)
//        {
//            var config = collect.ConfigList.FirstOrDefault(r => r.Field == field);
//            if (config.IsNull()) return string.Empty;

//            string result = string.Empty;
//            switch (config.Type)
//            {
//                case FieldType.选择器:
//                    result = GetHtmlContent(config, htmlString, index);
//                    break;
//                case FieldType.JSON:
//                    result = GetJsonValue(htmlString, config.JsonKey, index);
//                    break;
//                case FieldType.正则:
//                    result = GetRegex(config, htmlString, index);
//                    break;
//                case FieldType.XPATH:
//                    result = GetXpath(config, htmlString, index);
//                    break;
//                default:
//                    break;
//            }

//            //判断是否需要格式化拼接结果值
//            if (config.StringFormat.IsNotEmpty())
//            {
//                result = string.Format(config.StringFormat, result);
//            }

//            //替换
//            if (config.IsDisplace)
//            {
//                foreach (var item in displaceList)
//                {
//                    result = result.Replace(item.Word, item.NewWord);
//                }
//            }

//            //保存图片
//            if (config.SaveImg)
//            {
//                result = SaveImg(result);
//            }
//            return result;
//        }


//        /// <summary>
//        /// 保存图片
//        /// </summary>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        private static string SaveImg(string result)
//        {
//            try
//            {
//                Regex regex = new("<img\\s[^>]+>");
//                if (regex.IsMatch(result))
//                {
//                    var parser = new HtmlParser();
//                    var document = parser.ParseDocument(result);
//                    var imgList = document.QuerySelectorAll("img");
//                    foreach (var img in imgList)
//                    {
//                        var imgUrl = img.GetAttribute("src");
//                        var newSrc = DownloadImg(imgUrl);

//                        string pattern = @"src\s*=\s*(['""]?)([^'""]+)\1";
//                        string replacement = $"src=\"{newSrc}\"";
//                        var newHtml = new Regex(pattern).Replace(img.OuterHtml, replacement);


//                        result = result.Replace(img.OuterHtml, newHtml);
//                    }
//                }
//                else
//                {
//                    result = DownloadImg(result);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return result;
//        }


//        private static string DownloadImg(string imgUrl)
//        {
//            string extension = Path.GetExtension(imgUrl);
//            if (extension.Contains("?"))
//            {
//                extension = extension.Split("?")[0];
//            }

//            using WebClient client = new();
//            // 设置本地保存的路径
//            string fileName = $"{Id.NextId()}{extension}";
//            string saveFilePath = dir + $"/{fileName}";
//            client.DownloadFile(imgUrl, saveFilePath);
//            return $"/upload-files/{fileName}";
//        }

//        #endregion

//        #region 获取内容


//        /// <summary>
//        /// 获取html内容
//        /// </summary>
//        /// <param name="config"></param>
//        /// <param name="htmlString"></param>
//        /// <returns></returns>
//        private static string GetHtmlContent(CollectConfig config, string htmlString, int index)
//        {
//            var parser = new HtmlParser();
//            var document = parser.ParseDocument(htmlString);

//            var itemList = document.QuerySelectorAll($"{config.CharacterBegin} {config.CharacterEnd}");
//            if (config.Count) return itemList.Length.ToString();

//            if (config.JsonKey.IsNotEmpty())
//            {
//                string value = string.Empty;
//                try
//                {
//                    value = itemList[index].GetAttribute(config.JsonKey);
//                }
//                catch
//                {
//                    if (itemList.Length > 0)
//                    {
//                        value = itemList.FirstOrDefault().GetAttribute(config.JsonKey);
//                    }
//                }
//                return value;
//            }

//            if (config.SlotIndex.IsNotEmpty())
//            {
//                var i = config.SlotIndex.ToInt();
//                return config.InnerHtml ? itemList[i].InnerHtml : itemList[i].TextContent;
//            }

//            return config.InnerHtml ? itemList[index].InnerHtml : itemList[index].TextContent;
//        }


//        /// <summary>
//        ///  获取json键值
//        /// </summary>
//        /// <param name="str"></param>
//        /// <param name="key"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        private static string GetJsonValue(string str, string key, int index = 0)
//        {
//            string pattern = $"\"{key}\":\\s*\"?(?<value>[^\",{{}}]*)\"?,?";
//            var matches = Regex.Matches(str, pattern);
//            if (matches.Count == 0) return "";
//            try
//            {
//                return matches[index].Groups["value"].Value;
//            }
//            catch
//            {
//                return "";
//            }
//        }


//        /// <summary>
//        /// 正则获取
//        /// </summary>
//        /// <param name="config"></param>
//        /// <param name="htmlString"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public static string GetRegex(CollectConfig config, string htmlString, int index)
//        {
//            List<string> itemList = new();
//            string pattern = $@"{config.CharacterBegin}.*?{config.CharacterEnd}";
//            Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
//            MatchCollection matches = regex.Matches(htmlString);
//            foreach (Match match in matches)
//            {
//                itemList.Add(match.Value);
//            }

//            if (config.Count) return itemList.Count.ToString();

//            if (config.JsonKey.IsNotEmpty())
//            {
//                string value = string.Empty;
//                try
//                {
//                    string pattern1 = $@"(?<={config.JsonKey}=\"").*?(?=\"")";
//                    Regex regex1 = new(pattern1, RegexOptions.Singleline);
//                    Match match = regex1.Match(itemList[index]);
//                    value = match.Success ? match.Value : null;
//                }
//                catch
//                {
//                }
//                return value;
//            }

//            //if (config.SlotIndex.IsNotEmpty())
//            //{
//            //    var i = config.SlotIndex.ToInt();
//            //    string pattern = $@"{config.CharacterBegin}.*?{config.CharacterEnd}";
//            //    Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
//            //    MatchCollection matches = regex.Matches(htmlString);
//            //    foreach (Match match in matches)
//            //    {
//            //        itemList.Add(match.Value);
//            //    }
//            //}






//            return config.InnerHtml ? itemList[index] : Regex.Replace(itemList[index], @"<.*?>", string.Empty);

//        }


//        /// <summary>
//        /// Xpath获取
//        /// </summary>
//        /// <param name="config"></param>
//        /// <param name="htmlString"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        private static string GetXpath(CollectConfig config, string htmlString, int index)
//        {
//            var document = new HtmlDocument();
//            document.LoadHtml(htmlString);

//            var elements = document.DocumentNode.SelectNodes(config.CharacterBegin + config.CharacterEnd);
//            try
//            {
//                if (config.Count) return elements.Count.ToString();

//                if (config.JsonKey.IsNotEmpty())
//                {
//                    try
//                    {
//                        return elements[index].Attributes[config.JsonKey].Value;
//                    }
//                    catch
//                    {
//                        if (elements.Count > 0)
//                        {
//                            return elements.FirstOrDefault().Attributes[config.JsonKey].Value;
//                        }
//                    }
//                }

//                return config.InnerHtml ? elements[index].InnerHtml : elements[index].InnerText;
//            }
//            catch
//            {
//                return "";
//            }
//        }

//        #endregion
//    }
//}

