// "-----------------------------------------------------------------------
//  <copyright file="UserService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Response;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Application.Services.UserS
{
    public class UserService : BaseService, IUserService
    {
        private readonly ClientInfoHelper _clientInfoHelper;
        public UserService(IUnitOfWork unitOfWork, ClientInfoHelper clientInfoHelper) : base(unitOfWork)
        {
            _clientInfoHelper = clientInfoHelper;
        }

        public async Task<ApiResponse<object>> GetDeviceInfoAsync()
        {
            var res = new ApiResponse<object>()
            {
                Status = ResponseStatusCode.NotFound,
                Message = ResponseStatusName.NotFound,
            };

            var data = await UnitOfWork.RefreshToken.GetDevicesLoginByUser(_clientInfoHelper.GetUserID());
            if (data != null && data.Any())
            {
                res.Status = ResponseStatusCode.Success;
                res.Message = ResponseStatusName.Success;
                res.Data = data;
            }
            return res;
        }

        public Task<ApiResponse<object>> GetDevicesLoginAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> GetUserInfoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
