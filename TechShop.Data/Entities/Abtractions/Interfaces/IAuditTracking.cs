// "-----------------------------------------------------------------------
//  <copyright file="IAuditTracking.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace TechShop.Data.Entities.Abtractions.Interfaces
{
    public interface IAuditTracking : IDateTracking, IUserTracking
    {
    }
}
