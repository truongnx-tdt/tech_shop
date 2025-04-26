// "-----------------------------------------------------------------------
//  <copyright file="SessionValidationMiddleware.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.UnitOfWork;

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
                var sessionId = user.Claims.FirstOrDefault(c => c.Type == "session_id")?.Value;
                var username = user.Identity?.Name;

                if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(username))
                {
                    var token = await unitOfWork.RefreshToken.FindAsync(t =>
                        t.SessionId == sessionId && t.UserName == username);

                    if (token == null)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Phiên đăng nhập đã hết hạn.");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
