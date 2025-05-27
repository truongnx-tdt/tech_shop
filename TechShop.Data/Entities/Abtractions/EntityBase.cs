// "-----------------------------------------------------------------------
//  <copyright file="EntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Abtractions.Interfaces;

namespace TechShop.Data.Entities.Abtractions
{
    public class EntityBase<T> : IEntityBase<T>
    {
        public T Id { get; set; }
    }
}
