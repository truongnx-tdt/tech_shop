// "-----------------------------------------------------------------------
//  <copyright file="ISoftDelete.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"


namespace CrawlDataWebNews.Data.Entities.Abtractions.Interfaces
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}
