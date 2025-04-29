// "-----------------------------------------------------------------------
//  <copyright file="SessionValidationMiddleware.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.UnitOfWork;
using CrawlDataWebNews.Manufacture.CommonConst;

namespace CrawlDataWebNews.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var sessionId = user.Claims.FirstOrDefault(c => c.Type == StringConst.ClaimSessionId)?.Value;
                var username = user.Identity?.Name;

                if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(username))
                {
                    var token = await unitOfWork.RefreshToken.FindAsync(t =>
                        t.SessionId == sessionId && t.UserName == username);

                    if (token == null)
                    {
                        context.Response.StatusCode = ResponseStatusCode.Unauthorized;
                        await context.Response.WriteAsync(StringConst.SessionLoginEx);
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
