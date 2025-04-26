// "-----------------------------------------------------------------------
//  <copyright file="Helper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using NLog.Extensions.Logging;

namespace CrawlDataWebNews.Data.Common
{
    public class Helper
    {
        private static readonly ILogger<Helper> _logger;
        static Helper()
        {
            var loggerFactory = LoggerFactory.Create(cfg => cfg.AddNLog());
            _logger = loggerFactory.CreateLogger<Helper>();
        }

        private static HttpClient _httpClient;
        /// <summary>
        /// Get html from URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> InvokToUrlAsync(string url)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();

                // Thêm các header cần thiết để giả lập một trình duyệt web
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,vi;q=0.8");
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                _httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            }

            try
            {
                // Thực hiện yêu cầu
                var response = await _httpClient.GetAsync(url);

                // Kiểm tra mã trạng thái HTTP
                if (!response.IsSuccessStatusCode)
                {
                    // Nếu trạng thái không thành công (ví dụ: 404, 500), ghi lại lỗi và bỏ qua
                    _logger.LogError("URL bị lỗi" + url + " Mã lỗi:  " + response.StatusCode);
                    return null; // Trả về null nếu có lỗi
                }
                return response; // Trả về response nếu thành công
            }
            catch (HttpRequestException ex)
            {
                // Xử lý lỗi khi không thể thực hiện yêu cầu
                _logger.LogError($"Lỗi khi yêu cầu URL: {url}. Lỗi: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                _logger.LogError($"Lỗi không xác định khi yêu cầu URL: {url}. Lỗi: {ex.Message}");
            }

            return null; // Trả về null nếu có lỗi
        }
        /// <summary>
        ///  Phương thức để phân tích HTML và trích xuất các thẻ <a> có href bắt đầu bằng host hoặc "/"
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task<List<string>> ExtractLinksAsync(string htmlContent, string host, string extension, bool isContain)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(htmlContent));
            var links = new HashSet<string>(
                       document.QuerySelectorAll("a")
                           .Where(a => a.HasAttribute("href"))
                           .Select(a => a.GetAttribute("href"))
                           .Where(href => IsValidHref(href, host, extension, isContain))
                   );

            return links.ToList();
        }

        public static async Task<List<string>> GetLinksWithPlaywright(string url, string host, string extension, bool isContain)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);
            var anchors = await page.QuerySelectorAllAsync("a");

            var links = new List<string>();
            foreach (var anchor in anchors)
            {
                var href = await anchor.GetAttributeAsync("href");
                if (IsValidHref(href, host, extension, isContain))
                    links.Add(href);
            }

            await browser.CloseAsync();
            return links;
        }
        /// <summary>
        /// Kiểm tra href có hợp lệ hay không: bắt đầu với host hoặc "/" và không có phần mở rộng
        /// </summary>
        /// <param name="href"></param>
        /// <param name="host"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static bool IsValidHref(string href, string host, string extension, bool isContain)
        {
            if (string.IsNullOrWhiteSpace(href.Trim()))
                return false;
            if (!href.StartsWith(host) && !href.StartsWith("/"))
                return false;
            if (href.Equals("/"))
                return false;
            if (!string.IsNullOrWhiteSpace(extension))
            {
                List<string> listExten = extension.Split(';').Select(ext => ext.Trim()).ToList();
                if (isContain)
                {
                    return listExten.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return !listExten.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
                }
            }
            else
            {
                return !href.Substring(href.LastIndexOf("/") + 1).Contains(".");
            }
        }
        public static async Task<string> ReadContentAsync(HttpResponseMessage response)
        {
            var contentEncoding = response.Content.Headers.ContentEncoding;
            var stream = await response.Content.ReadAsStreamAsync();
            if (contentEncoding.Contains("gzip"))
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(gzipStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else if (contentEncoding.Contains("deflate"))
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(deflateStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else if (contentEncoding.Contains("br"))
            {
                using (var brotliStream = new BrotliStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(brotliStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> ExtractTitle(string htmlContent)
        {
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(htmlContent));
            var titleElement = document.QuerySelector("title");
            return titleElement?.TextContent?.Trim();
        }
        public static async Task<string> ExtractAuthor(string htmlContent)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlContent);
            var authorElements = document.QuerySelectorAll("[class*='autho']");
            foreach (var element in authorElements)
            {
                return element.TextContent.Trim();
            }
            return "";
        }
        public static async Task<string> ExtractContent(string htmlContent)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlContent);
            var contentDivs = document.QuerySelectorAll("div[class*='container'], div[class*='content'], div[ng-controller]");
            foreach (var div in contentDivs)
            {
                var h1Element = div.QuerySelector("h1");
                if (h1Element != null)
                {
                    return div.TextContent.Trim();
                }
            }
            return "";
        }
        public static async Task<string> ExtractContentHtml(string htmlContent)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlContent);
            var contentDivs = document.QuerySelectorAll("div[class*='container'], div[class*='content'], div[ng-controller]");
            foreach (var div in contentDivs)
            {
                var h1Element = div.QuerySelector("h1");
                if (h1Element != null)
                {
                    return div.OuterHtml.Trim();
                }
            }
            return "";
        }
    }
}
