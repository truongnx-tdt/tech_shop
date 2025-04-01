using CrawlDataWebNews.Data.Response;

namespace CrawlDataWebNews.Application.Services.Interfaces
{
    public interface IGetDataService 
    {
        Task<ICollection<CategoriesResponse>> GetData(string url);
        Task<ICollection<CategoriesResponse>> GetByCtg(string url, string extension);
    }
}
