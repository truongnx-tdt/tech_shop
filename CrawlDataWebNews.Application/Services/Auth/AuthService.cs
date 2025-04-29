// "-----------------------------------------------------------------------
//  <copyright file="AuthService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CrawlDataWebNews.Application.Services.Token;
using CrawlDataWebNews.Data.DTO;
using CrawlDataWebNews.Data.Entities.Auth;
using CrawlDataWebNews.Data.Request;
using CrawlDataWebNews.Data.Response;
using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.UnitOfWork;
using CrawlDataWebNews.Manufacture;
using CrawlDataWebNews.Manufacture.CommonConst;
using CrawlDataWebNews.Manufacture.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrawlDataWebNews.Application.Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly ClientInfoHelper _clientInfoHelper;
        private readonly ITokenService _tokenService;
        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, ClientInfoHelper clientInfoHelper, ITokenService tokenService) : base(unitOfWork)
        {
            _logger = logger;
            _clientInfoHelper = clientInfoHelper;
            _tokenService = tokenService;
        }
        /// <summary>
        /// Do login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<LoginResponse> Login(LoginRequest model)
        {
            var rs = new LoginResponse();
            try
            {
                var userExists = await UnitOfWork.UserRepository
                                        .FindAsync(x => x.Username.Trim().ToLower() == model.Account.Trim() || x.Email.Trim().ToLower() == model.Account.Trim());
                if (userExists != null && PasswordHasher.VerifyPassword(model.Password, userExists.PasswordHash))
                {
                    userExists.LastLogin = DateTimeOffset.UtcNow;
                    string sessionId = Guid.NewGuid().ToString();

                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userExists.Username),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Email, userExists.Email),
                            new Claim(StringConst.ClaimUserId, userExists.Id.ToString()),
                            new Claim(StringConst.ClaimRole, userExists.Role),
                            new Claim(StringConst.ClaimSessionId, sessionId),
                        };
                    string token = _tokenService.GenerateAccessToken(authClaims);
                    string refreshToken = _tokenService.GenerateRefreshToken();
                    #region save refresh token to db
                    string device = _clientInfoHelper.GetUserAgent();
                    string ip = _clientInfoHelper.GetClientIp();
                    var ti = new RefreshToken
                    {
                        UserName = userExists.Username,
                        UserId = userExists.Id,
                        Token = refreshToken,
                        ExpiredAt = DateTimeOffset.UtcNow.AddDays(AppSettings.RefreshTokenExperyTimeInDay),
                        DeviceInfo = device,
                        IpAddress = ip,
                        SessionId = sessionId
                    };
                    UnitOfWork.RefreshToken.Add(ti);
                    await UnitOfWork.CommitAsync();
                    #endregion
                    rs.AccessToken = token;
                    rs.RefeshToken = refreshToken;
                    rs.IsLogin = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new UnauthorizedAccessException(StringConst.Exception);
            }
            return rs;
        }

        public async Task<(bool, string)> LogoutAllAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return (false, StringConst.LogoutFailed);
            }
            var rs = false;
            var createStrategy = UnitOfWork.CreateExecutionStrategy();
            await createStrategy.ExecuteAsync(async () =>
            {
                using (var db = await UnitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var allTokens = await UnitOfWork.RefreshToken.FindByAsyn(t => t.UserName == username);
                        await UnitOfWork.BulkDeleteAsync<RefreshToken>(allTokens.ToList()); // using bulk delete to improve performance, not need save change EF
                        rs = true;
                        if (rs)
                        {
                            await db.CommitAsync();
                        }
                        else
                        {
                            await db.RollbackAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await db.RollbackAsync();
                        _logger.LogError(ex.ToString());
                        throw new UnauthorizedAccessException(StringConst.Exception);
                    }
                }
            });
            return (rs, rs ? StringConst.Logout : StringConst.LogoutFailed);
        }

        public async Task<(bool, string)> LogoutAsync(string username, string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(sessionId))
                    throw new UnauthorizedAccessException(StringConst.MissInformation);

                var token = await UnitOfWork.RefreshToken.FindAsync(t =>
                    t.UserName == username && t.SessionId == sessionId);

                if (token != null)
                {
                    await UnitOfWork.RefreshToken.DeleteAsyn(token);
                    await UnitOfWork.CommitAsync();
                    return (true, StringConst.Logout);
                }
                return (false, StringConst.LogoutFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new UnauthorizedAccessException(StringConst.Exception);
            }
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
                                PasswordHash = passwordHash,
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
        /// funtion service for refresh token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TokenModel> TokenRefresh(TokenModel model)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
                var username = principal.Identity?.Name;
                var sessionId = principal.Claims.FirstOrDefault(c => c.Type == StringConst.ClaimSessionId)?.Value;

                var tokenInfo = await UnitOfWork.RefreshToken.FindAsync(u => u.UserName == username && u.SessionId == sessionId);
                if (tokenInfo == null
                    || tokenInfo.Token != model.RefreshToken
                    || tokenInfo.ExpiredAt <= DateTimeOffset.UtcNow)
                {
                    throw new Exception(StringConst.InvalidToken);
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.Token = newRefreshToken;
                await UnitOfWork.CommitAsync();

                return (new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(StringConst.Exception);
            }
        }
    }
}
