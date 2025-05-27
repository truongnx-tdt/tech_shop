// "-----------------------------------------------------------------------
//  <copyright file="ITokenService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Security.Claims;

namespace TechShop.Application.Services.Token
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> authClaims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    }
}
