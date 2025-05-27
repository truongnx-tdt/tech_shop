// "-----------------------------------------------------------------------
//  <copyright file="AuthService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TechShop.Application.Services.Token;
using TechShop.Data.DTO;
using TechShop.Data.Entities.Auth;
using TechShop.Data.Request;
using TechShop.Data.Response;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture;
using TechShop.Manufacture.CommonConst;
using TechShop.Manufacture.Utils;
using TechShop.Data.Entities.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TechShop.Application.Services.Auth
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
                                        .FindAsync(x => (x.Username.Trim().ToLower() == model.Account.Trim() || x.Email.Trim().ToLower() == model.Account.Trim()) && x.LoginProvider == StringConst.LoginProviderDefault);
                if (userExists != null && PasswordHasher.VerifyPassword(model.Password, userExists.PasswordHash))
                {
                    if (!userExists.IsActive)
                    {
                        rs.Message = StringConst.UserNotActive;
                        return rs;
                    }
                    userExists.LastLogin = DateTimeOffset.UtcNow;
                    string sessionId = Guid.NewGuid().ToString();

                    var authClaims = CreateClams(userExists, sessionId);
                    string token = _tokenService.GenerateAccessToken(authClaims);
                    string refreshToken = _tokenService.GenerateRefreshToken();
                    await SaveRefeshToken(userExists, refreshToken, sessionId);
                    rs.AccessToken = token;
                    rs.RefeshToken = refreshToken;
                    rs.Message = StringConst.LoginIn;
                    return rs;
                }
                rs.Message = StringConst.UserOrPwdIncorrect;
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
                        await UnitOfWork.BulkDeleteAsync(allTokens.ToList()); // using bulk delete to improve performance, not need save change EF
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
                    await UnitOfWork.SaveChangesAsync();
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
                        var userExists = await UnitOfWork.UserRepository.FindAsync(x => (x.Username.Trim().ToLower() == model.Username.Trim().ToLower() || x.Email.Trim().ToLower() == model.Email.Trim().ToLower()) && x.LoginProvider == StringConst.LoginProviderDefault);
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
                            rs = await UnitOfWork.SaveChangesAsync();
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
                await UnitOfWork.SaveChangesAsync();

                return new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(StringConst.Exception);
            }
        }
        private List<Claim> CreateClams(User userExists, string sessionId)
        {
            return new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userExists.Username),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Email, userExists.Email),
                            new Claim(StringConst.ClaimUserId, userExists.Id.ToString()),
                            new Claim(StringConst.ClaimRole, userExists.Role),
                            new Claim(StringConst.ClaimSessionId, sessionId),
                        };
        }
        private async Task SaveRefeshToken(User userExists, string refreshToken, string sessionId)
        {
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
            await UnitOfWork.SaveChangesAsync();
            #endregion
        }
        #region handle login google
        public async Task<LoginResponse> GoogleLogin([FromBody] string idToken)
        {
            try
            {
                var payload = await ValidateGoogleTokenAsync(idToken);
                var user = await UnitOfWork.UserRepository
                    .FirstOrDefaultAsync(u => u.Email == payload.Email && u.LoginProvider == StringConst.LoginProviderGG);
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        Username = payload.Email,
                        FullName = payload.Name,
                        Picture = payload.Picture,
                        GoogleId = payload.Subject,
                        PasswordHash = "",
                        LoginProvider = StringConst.LoginProviderGG,
                        IsActive = true
                    };
                    await UnitOfWork.UserRepository.AddAsyn(user);
                    await UnitOfWork.SaveChangesAsync();
                }

                // Sinh JWT hoặc Refresh Token như thông thường
                string sessionId = Guid.NewGuid().ToString();

                var authClaims = CreateClams(user, sessionId);
                string token = _tokenService.GenerateAccessToken(authClaims);
                string refreshToken = _tokenService.GenerateRefreshToken();
                
                await SaveRefeshToken(user, refreshToken, sessionId);

                return new LoginResponse() { AccessToken = token, RefeshToken = refreshToken, Message = StringConst.LoginIn };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            if (payload == null || string.IsNullOrEmpty(payload.Email))
                throw new UnauthorizedAccessException("Invalid Google Token");

            return payload;
        }
        private async Task<User> GetGoogleUserProfileAsync(string accessToken)
        {
            using var httpClient = new HttpClient();

            // Send GET request to Google UserInfo API
            var response = await httpClient.GetAsync($"{AppSettings.GoogleUserInfoUrl}?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            // Parse the JSON response
            var content = await response.Content.ReadAsStringAsync();
            var userProfile = JsonConvert.DeserializeObject<User>(content);

            return userProfile!;
        }
        #endregion
    }
}
