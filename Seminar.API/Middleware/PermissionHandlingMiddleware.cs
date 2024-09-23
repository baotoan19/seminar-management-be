// using Seminar.APPLICATION.Auth;

// namespace Seminar.API.Middleware;

// public class PermissionHandlingMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ILogger<PermissionHandlingMiddleware> _logger;
//     private readonly Dictionary<string, List<string>> _rolePermissions;
//     private readonly IEnumerable<string> _excludedUris;

//     public PermissionHandlingMiddleware(RequestDelegate next, ILogger<PermissionHandlingMiddleware> logger)
//     {
//         _next = next;
//         _logger = logger;
//     }

//     public async Task Invoke(HttpContext context)
//     {
//         if (!HasPermission(context))
//         {
//             await Authentication.HandleForbiddenRequest(context);
//             return;
//         }

//         await _next(context);
//     }

//     private bool HasPermission(HttpContext context)
//     {
//         string requestUri = context.Request.Path.Value!;

//         if (_excludedUris.Contains(requestUri) || !requestUri.StartsWith("/api/"))
//             return true;

//         string[] segments = requestUri.Split('/');

//         string featureUri = string.Join("/", segments.Take(segments.Length - 1));

//         string controller = segments.Length > 2 ? $"/api/{segments[2]}" : string.Empty;

//         try
//         {
//             string userRole = Authentication.GetUserRoleFromHttpContext(context);

//             //// If the user role is admin, allow access to all controllers
//             if (userRole == "admin") return true;

//             // Check if the user's role has permission to access the requested controller
//             if (_rolePermissions.TryGetValue(userRole, out var allowedControllers))
//             {
//                 return allowedControllers.Any(uri => requestUri.StartsWith(uri, System.StringComparison.OrdinalIgnoreCase));
//             }
//             return false;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error while checking permissions");
//             return false;
//         }
//     }
// }