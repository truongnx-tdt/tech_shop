// "-----------------------------------------------------------------------
//  <copyright file="UserRole.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace TechShop.Manufacture.Enums
{
    public enum UserRole
    {
        admin = 1,
        user = 2, // default role for new users
    }

    public static class UserRoleNames
    {
        public const string Admin = nameof(UserRole.admin);
        public const string User = nameof(UserRole.user);
    }

}
