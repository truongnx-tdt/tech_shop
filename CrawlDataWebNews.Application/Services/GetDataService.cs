using System.Collections.Generic;
using AngleSharp.Css;
using AngleSharp.Io;
using CrawlDataWebNews.Application.Services.Interfaces;
using CrawlDataWebNews.Data.Common;
using CrawlDataWebNews.Data.Response;

namespace CrawlDataWebNews.Application.Services
{
    public class GetDataService : IGetDataService
    {
        public async Task<ICollection<CategoriesResponse>> GetByCtg(string url, string extension)
        {
            var rs = new List<CategoriesResponse>();
            var hrefDetailList = new HashSet<string>();
            var articles = new List<Article>();
            var tasks = new List<Task>();
            var task = Task.Run(async () =>
            {
                var ortherPage = await Helper.InvokToUrlAsync(url);
                if (ortherPage != null)
                {
                    string subHtml = await Helper.ReadContentAsync(ortherPage);
                    var childrenLinks = await Helper.ExtractLinksAsync(subHtml, url, extension, true);
                    var articles = new List<Article>();
                    var cate = new CategoriesResponse
                    {
                        Title = await Helper.ExtractTitle(subHtml),
                        Href = url,
                    };

                    foreach (var subLink in childrenLinks)
                    {
                        string htmlUrl = subLink.StartsWith(url) ? subLink : url + subLink;
                        if (!hrefDetailList.Contains(htmlUrl))
                        {
                            hrefDetailList.Add(htmlUrl);
                            articles.Add(new Article
                            {
                                Href = htmlUrl,
                                Id = Guid.NewGuid(),
                            });
                        }
                    }
                    cate.Articles = articles;
                    rs.Add(cate);
                }
            });
            tasks.Add(task);

            await Task.WhenAll(tasks);
            tasks.Clear();

            int batchSize = 5;
            tasks.Clear();
            foreach (var linkDetail in hrefDetailList)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var ortherPage = await Helper.InvokToUrlAsync(linkDetail);
                    if (ortherPage.IsSuccessStatusCode)
                    {
                        string subHtml = await Helper.ReadContentAsync(ortherPage);
                        var category = rs.FirstOrDefault(c => c.Articles.Any(a => a.Href == linkDetail));
                        if (category != null)
                        {
                            var existingArticle = category.Articles.FirstOrDefault(a => a.Href == linkDetail);
                            if (existingArticle != null)
                            {
                                existingArticle.Title = await Helper.ExtractTitle(subHtml);
                                existingArticle.Content = await Helper.ExtractContent(subHtml);
                                existingArticle.ContentHtml = await Helper.ExtractContentHtml(subHtml);
                                existingArticle.Author = await Helper.ExtractAuthor(existingArticle.ContentHtml);

                                // Kiểm tra nếu content rỗng, xóa article
                                if (string.IsNullOrEmpty(existingArticle.Content))
                                {
                                    category.Articles.Remove(existingArticle); // Xóa article khỏi category
                                }
                            }

                        }
                    }
                }));
                if (tasks.Count >= batchSize)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            await Task.WhenAll(tasks);
            return rs;
        }

        public async Task<ICollection<CategoriesResponse>> GetData(string url)
        {
            var rs = new List<CategoriesResponse>();
            var pagesToVisit = new Queue<string>();
            var hrefDetailList = new HashSet<string>();
            var visitedUrls = new HashSet<string>();
            var articles = new List<Article>();
            pagesToVisit.Enqueue(url);
            var batchSize = 10;
            var tasks = new List<Task>();

            while (pagesToVisit.Count > 0)
            {
                while (tasks.Count < batchSize && pagesToVisit.Count > 0)
                {
                    var currentUrl = pagesToVisit.Dequeue();
                    if (visitedUrls.Contains(currentUrl))
                    {
                        continue;
                    }
                    visitedUrls.Add(currentUrl);
                    var task = Task.Run(async () =>
                    {
                        var ortherPage = await Helper.InvokToUrlAsync(currentUrl);
                        if (ortherPage != null)
                        {
                            string subHtml = await Helper.ReadContentAsync(ortherPage);
                            var childrenLinks = await Helper.ExtractLinksAsync(subHtml, url, "", false);

                            rs.Add(new CategoriesResponse
                            {
                                Title = await Helper.ExtractTitle(subHtml),
                                Href = currentUrl,
                            });

                            foreach (var subLink in childrenLinks)
                            {
                                string htmlUrl = subLink.StartsWith(url) ? subLink : url + subLink;
                                if (!hrefDetailList.Contains(htmlUrl))
                                {
                                    hrefDetailList.Add(htmlUrl);
                                    if (!visitedUrls.Contains(htmlUrl))
                                    {
                                        pagesToVisit.Enqueue(htmlUrl);
                                    }
                                }
                            }
                        }
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
            return rs;
        }
    }
}
