// "-----------------------------------------------------------------------
//  <copyright file="IGetDataService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Response;

namespace TechShop.Application.Services.Interfaces
{
    public interface IGetDataService 
    {
        Task<ICollection<CategoriesResponse>> GetData(string url);
        Task<CategoriesResponse> GetByCtg(string url, string extension);
    }
}
