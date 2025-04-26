// "-----------------------------------------------------------------------
//  <copyright file="IDateTracking.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace CrawlDataWebNews.Data.Entities.Abtractions.Interfaces
{
    public interface IDateTracking
    {
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset UpdateAt { get; set; }
    }
}
