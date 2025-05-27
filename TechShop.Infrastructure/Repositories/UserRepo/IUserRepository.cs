// "-----------------------------------------------------------------------
//  <copyright file="IUserRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Auth;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.UserRepo
{
    public interface IUserRepository : IGenericRepository<User>
    {

    }
}
