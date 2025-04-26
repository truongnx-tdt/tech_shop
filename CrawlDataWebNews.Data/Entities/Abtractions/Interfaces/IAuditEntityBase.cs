// "-----------------------------------------------------------------------
//  <copyright file="IAuditEntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace CrawlDataWebNews.Data.Entities.Abtractions.Interfaces
{
    public interface IAuditEntityBase<T> : IEntityBase<T>, IAuditTracking
    {
    }
}
