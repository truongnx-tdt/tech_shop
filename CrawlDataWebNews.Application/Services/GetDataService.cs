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
        public async Task<CategoriesResponse> GetByCtg(string url, string extension)
        {
            var uri = new Uri(url);
            string currentUrl = uri.Scheme + "://" + uri.Host;
            var rs = new CategoriesResponse();
            var hrefDetailList = new HashSet<string>();
            var articles = new List<Article>();
            var tasks = new List<Task>();

            var ortherPage = await Helper.InvokToUrlAsync(url);
            if (ortherPage != null)
            {
                string subHtml = await Helper.ReadContentAsync(ortherPage);
                var childrenLinks = await Helper.ExtractLinksAsync(subHtml, currentUrl, extension, true);
                var cate = new CategoriesResponse
                {
                    Title = await Helper.ExtractTitle(subHtml),
                    Href = url,
                };

                foreach (var subLink in childrenLinks)
                {
                    string htmlUrl = subLink.StartsWith(currentUrl) ? subLink : currentUrl + subLink;
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
                rs = cate;
            }



            int batchSize = 5;
            tasks.Clear();
            foreach (var linkDetail in hrefDetailList)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var ortherPage = await Helper.InvokToUrlAsync(linkDetail);
                    if (ortherPage != null)
                    {
                        string subHtml = await Helper.ReadContentAsync(ortherPage);

                        var existingArticle = rs.Articles.FirstOrDefault(a => a.Href == linkDetail);
                        if (existingArticle != null)
                        {
                            existingArticle.Title = await Helper.ExtractTitle(subHtml);
                            existingArticle.Content = await Helper.ExtractContent(subHtml);
                            existingArticle.ContentHtml = await Helper.ExtractContentHtml(subHtml);
                            existingArticle.Author = await Helper.ExtractAuthor(existingArticle.ContentHtml);
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
            if (rs.Articles != null)
            {
                rs.Articles.RemoveAll(article =>
                    string.IsNullOrEmpty(article.ContentHtml)
                   );
            }
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
