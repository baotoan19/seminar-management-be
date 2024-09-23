
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NLog.LayoutRenderers;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;

namespace Seminar.APPLICATION.Auth;

public class Authentication
{
    public static string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            if (httpContextAccessor.HttpContext == null || !httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new UnauthorizedAccessException("Need Authorization");
            }

            string? authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Invalid Authorization Header: {authorizationHeader}");
            }
            string jwtToken = authorizationHeader["Bearer ".Length..].Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(jwtToken))
            {
                throw new UnauthorizedAccessException("Invalid token format");
            }
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");
            return idClaim?.Value ?? throw new UnauthorizedAccessException("User id not found in token");
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorResponse = new
            {
                data = "An unexpected error occurred.",
                message = ex.Message,
                statusCode = StatusCodes.Status401Unauthorized,
                code = "Unauthorized!"
            };
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            if (httpContextAccessor.HttpContext != null)
            {
                httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                httpContextAccessor.HttpContext.Response.WriteAsync(jsonResponse).Wait();
            }
            httpContextAccessor.HttpContext?.Response.WriteAsync(jsonResponse).Wait();
            throw;
        }
    }

    public static string GetUserRoleFromHttpContext(HttpContext httpContext)
    {
        try
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new UnauthorizedException("Need Authorization");
            }

            string? authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");
            }

            string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(jwtToken))
            {
                throw new UnauthorizedException("Invalid token format");
            }

            var token = tokenHandler.ReadJwtToken(jwtToken);
            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "role");

            return roleClaim?.Value ?? throw new UnauthorizedException("Cannot get user id from token");
        }
        catch (UnauthorizedException ex)
        {
            var errorResponse = new
            {
                data = "An unexpected error occurred.",
                message = ex.Message,
                statusCode = StatusCodes.Status401Unauthorized,
                code = "Unauthorized!"
            };

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.WriteAsync(jsonResponse).Wait();

            throw; // Re-throw the exception to maintain the error flow
        }
    }

    public static async Task HandleForbiddenRequest(HttpContext context)
    {
        int code = (int)HttpStatusCode.Forbidden;
        var error = new ErrorException(code, ResponseCodeConstants.FORBIDDEN, "You don't have permission to access this feature");
        string result = JsonSerializer.Serialize(error);

        context.Response.ContentType = "application/json";
        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        context.Response.StatusCode = code;

        await context.Response.WriteAsync(result);
    }

}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}