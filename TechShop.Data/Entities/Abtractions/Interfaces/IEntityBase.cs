// "-----------------------------------------------------------------------
//  <copyright file="IEntityBase.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace TechShop.Data.Entities.Abtractions.Interfaces
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
