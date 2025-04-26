// "-----------------------------------------------------------------------
//  <copyright file="AuthService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrawlDataWebNews.Data.Entities.Auth;
using CrawlDataWebNews.Data.Request;
using CrawlDataWebNews.Data.Response;
using CrawlDataWebNews.Infrastructure.UnitOfWork;
using CrawlDataWebNews.Manufacture;
using CrawlDataWebNews.Manufacture.CommonConst;
using CrawlDataWebNews.Manufacture.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CrawlDataWebNews.Application.Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        public async Task<LoginResponse> Login(LoginRequest model)
        {
            var rs = new LoginResponse();
            try
            {
                var userExists = await UnitOfWork.UserRepository
                                        .FindAsync(x => x.Username.Trim().ToLower() == model.Account.Trim() || x.Email.Trim().ToLower() == model.Account.Trim());

                if (userExists != null && PasswordHasher.VerifyPassword(model.Password, userExists.PasswordHash))
                {
                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userExists.Username),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Email, userExists.Email),
                            new Claim("UserID", userExists.Id.ToString()),
                            new Claim("Roles", userExists.Role),
                        };
                    string token = GenerateAccessToken(authClaims);
                    rs.AccessToken = token;
                    rs.IsLogin = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return rs;
        }
        /// <summary>
        /// Function Service for create new account user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(int, string)> Registeration(RegistrationRequest model)
        {
            var createStrategy = UnitOfWork.CreateExecutionStrategy();
            var rs = false;
            var response = (ResponseStatusCode.UnSuccess, ResponseStatusName.UnSuccess);
            await createStrategy.ExecuteAsync(async () =>
            {
                using (var db = await UnitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var userExists = await UnitOfWork.UserRepository.FindAsync(x => x.Username.Trim().ToLower() == model.Username.Trim().ToLower() || (x.Email.Trim().ToLower() == model.Email.Trim().ToLower()));
                        if (userExists == null)
                        {
                            var passwordHash = PasswordHasher.HashPassword(model.Password);
                            User user = new()
                            {
                                Email = model.Email,
                                Username = model.Username,
                                FullName = model.FullName,
                                PasswordHash = model.Password,
                            };
                            await UnitOfWork.UserRepository.AddAsyn(user);
                            rs = await UnitOfWork.CommitAsync();
                            if (rs)
                            {
                                response.Item1 = ResponseStatusCode.Success;
                                response.Item2 = ResponseStatusName.Success;
                                await db.CommitAsync();
                            }
                            else
                            {
                                await db.RollbackAsync();
                            }
                        }
                        else
                        {
                            response.Item1 = ResponseStatusCode.Exists;
                            response.Item2 = ResponseStatusName.Exists;
                        }
                    }
                    catch (Exception ex)
                    {
                        await db.RollbackAsync();
                        _logger.LogError(ex.Message);
                    }
                }
            });
            return response;
        }

        /// <summary>
        /// Function generate access token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Access Token</returns>
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = AppSettings.ValidIssuer,
                Audience = AppSettings.ValidAudience,
                Expires = DateTime.UtcNow.AddMinutes(AppSettings.TokenExpiryTiemInMinutes),
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandle = new JwtSecurityTokenHandler();
            var token = tokenHandle.CreateToken(tokenDescriptor);
            return tokenHandle.WriteToken(token);
        }
    }
}
