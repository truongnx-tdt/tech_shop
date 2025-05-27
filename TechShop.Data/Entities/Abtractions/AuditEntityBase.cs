// "-----------------------------------------------------------------------
//  <copyright file="AuditEntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using TechShop.Data.Entities.Abtractions.Interfaces;

namespace TechShop.Data.Entities.Abtractions
{
    public abstract class AuditEntityBase<T> : IAuditEntityBase<T>
    {
        [Key]
        public T Id { get; set; } 
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset UpdateAt { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
    }
}
