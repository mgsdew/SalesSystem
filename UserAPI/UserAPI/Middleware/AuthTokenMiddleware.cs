namespace UserAPI.Middleware;

/// <summary>
/// Middleware for validating authentication tokens in API requests.
/// </summary>
public class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthTokenMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public AuthTokenMiddleware(RequestDelegate next, ILogger<AuthTokenMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for health check and swagger endpoints
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && (path.Contains("/health") ||
                            path.Contains("/swagger") ||
                            path.Contains("/swagger.json")))
        {
            await _next(context);
            return;
        }

        // Check for Authorization header
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            _logger.LogWarning("Request made without Authorization header from {IpAddress}",
                context.Connection.RemoteIpAddress);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Authorization header is missing\"}");
            return;
        }

        // Extract token from header
        var token = authHeader.ToString();

        // Remove "Bearer " prefix if present
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token.Substring("Bearer ".Length).Trim();
        }

        // Validate token
        if (!ValidateToken(token))
        {
            _logger.LogWarning("Invalid token attempt from {IpAddress}",
                context.Connection.RemoteIpAddress);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Invalid or expired token\"}");
            return;
        }

        _logger.LogInformation("Request authorized successfully for {Path}", context.Request.Path);

        // Token is valid, proceed to the next middleware
        await _next(context);
    }

    private bool ValidateToken(string token)
    {
        // Get valid tokens from environment variable first, fall back to configuration
        var validTokensString = Environment.GetEnvironmentVariable("AUTH_VALID_TOKENS");

        string[]? validTokens = null;

        if (!string.IsNullOrEmpty(validTokensString))
        {
            // Parse tokens from environment variable (semicolon-separated)
            validTokens = validTokensString.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(t => t.Trim())
                                          .Where(t => !string.IsNullOrEmpty(t))
                                          .ToArray();
        }
        else
        {
            // Fall back to configuration from appsettings
            validTokens = _configuration.GetSection("Authentication:ValidTokens").Get<string[]>();
        }

        if (validTokens == null || validTokens.Length == 0)
        {
            _logger.LogWarning("No valid tokens configured. Set AUTH_VALID_TOKENS environment variable or configure in appsettings");
            return false;
        }

        // Check if the provided token matches any valid token
        return validTokens.Any(validToken =>
            string.Equals(token, validToken, StringComparison.Ordinal));
    }
}

/// <summary>
/// Extension methods for adding the AuthTokenMiddleware to the application pipeline.
/// </summary>
public static class AuthTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthTokenMiddleware>();
    }
}