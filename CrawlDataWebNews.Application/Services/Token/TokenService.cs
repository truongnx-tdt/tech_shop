// "-----------------------------------------------------------------------
//  <copyright file="TokenService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CrawlDataWebNews.Manufacture;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace CrawlDataWebNews.Application.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        public TokenService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Function generate access token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Access Token</returns>
        public string GenerateAccessToken(IEnumerable<Claim> authClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = AppSettings.ValidIssuer,
                Audience = AppSettings.ValidAudience,
                Expires = DateTime.UtcNow.AddMinutes(AppSettings.TokenExpiryTiemInMinutes),
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(authClaims)
            };

            var tokenHandle = new JwtSecurityTokenHandler();
            var token = tokenHandle.CreateToken(tokenDescriptor);
            return tokenHandle.WriteToken(token);
        }
        /// <summary>
        /// Function generate refresh token
        /// </summary>
        /// <returns>Refresh Token</returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        /// <summary>
        /// Funtion get principal from expired token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            // Define the token validation parameters used to validate the token.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = AppSettings.ValidAudience,
                ValidIssuer = AppSettings.ValidIssuer,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey
                           (Encoding.UTF8.GetBytes(AppSettings.SecretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token and extract the claims principal and the security token.
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            // Cast the security token to a JwtSecurityToken for further validation.
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            // Ensure the token is a valid JWT and uses the HmacSha256 signing algorithm.
            // If no throw new SecurityTokenException
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals
            (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogError("Invalid token: {token}", accessToken);
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
