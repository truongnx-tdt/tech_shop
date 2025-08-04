// "-----------------------------------------------------------------------
//  <copyright file="IUserService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Response;

namespace TechShop.Application.Services.UserS
{
    public interface IUserService
    {
        Task<ApiResponse<object>> GetDevicesLoginAsync();
        Task<ApiResponse<object>> GetDeviceInfoAsync();
        Task<ApiResponse<object>> GetUserInfoAsync();
    }
}
