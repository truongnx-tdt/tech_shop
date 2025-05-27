// "-----------------------------------------------------------------------
//  <copyright file="RefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.RefreshTokenRepo
{
    public class RefreshTokenRepository : GenericRepository<Data.Entities.Auth.RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
