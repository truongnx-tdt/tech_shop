// "-----------------------------------------------------------------------
//  <copyright file="RefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.EntityFrameworkCore;
using TechShop.Data.Entities.Auth;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.RefreshTokenRepo
{
    public class RefreshTokenRepository : GenericRepository<Data.Entities.Auth.RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RefreshToken>> GetDevicesLoginByUser(string user)
        {
            IQueryable<RefreshToken> query = _context.RefreshTokens
                .Where(rt => rt.UserId.ToString() == user || rt.UserName.Equals(user))
                .Select(rt => new RefreshToken
                {
                    Id = rt.Id,
                    UserName = rt.UserName,
                    DeviceInfo = rt.DeviceInfo,
                    IpAddress = rt.IpAddress,
                    SessionId = rt.SessionId
                })
                .AsNoTracking();
            return await query.ToListAsync().ConfigureAwait(false);
        }
    }
}
