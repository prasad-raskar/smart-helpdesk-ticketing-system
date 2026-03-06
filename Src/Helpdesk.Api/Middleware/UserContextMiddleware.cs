using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;

namespace Helpdesk.Api.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User?.FindFirst("id")?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
                return;
            }
        }

        await _next(context);
    }
}
