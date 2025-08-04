// "-----------------------------------------------------------------------
//  <copyright file="AddressRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.AddressRepo
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
