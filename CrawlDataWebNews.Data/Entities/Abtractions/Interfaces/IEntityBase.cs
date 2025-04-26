// "-----------------------------------------------------------------------
//  <copyright file="IEntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace CrawlDataWebNews.Data.Entities.Abtractions.Interfaces
{
    /// <summary>
    /// T is data type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityBase<T>
    {
       public T Id { get; set;}
    }
}
