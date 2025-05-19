// "-----------------------------------------------------------------------
//  <copyright file="EntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Abtractions.Interfaces;

namespace CrawlDataWebNews.Data.Entities.Abtractions
{
    public class EntityBase<T> : IEntityBase<T>
    {
        public T Id { get; set; }
    }
}
