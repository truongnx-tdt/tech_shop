// "-----------------------------------------------------------------------
//  <copyright file="Address.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Abtractions;
using TechShop.Data.Entities.Auth;

namespace TechShop.Data.Entities
{
    public class Address : EntityBase<int>
    {
        public string Details { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
