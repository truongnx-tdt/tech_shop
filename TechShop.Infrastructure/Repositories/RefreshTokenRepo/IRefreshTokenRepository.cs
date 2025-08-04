// "-----------------------------------------------------------------------
//  <copyright file="IRefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"


// "-----------------------------------------------------------------------
//  <copyright file="IRefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Auth;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.RefreshTokenRepo
{
    public interface IRefreshTokenRepository : IGenericRepository<Data.Entities.Auth.RefreshToken>
    {
        Task<IEnumerable<RefreshToken>> GetDevicesLoginByUser(string user);
    }
}
