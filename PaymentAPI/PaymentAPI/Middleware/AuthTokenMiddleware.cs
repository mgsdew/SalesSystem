namespace PaymentAPI.Middleware;

/// <summary>
/// Middleware for validating authentication tokens with user context.
/// </summary>
public class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthTokenMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthTokenMiddleware(RequestDelegate next, ILogger<AuthTokenMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
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

        // Validate token with UserAPI
        var userInfo = await ValidateTokenWithUserAPI(token);
        if (userInfo == null)
        {
            _logger.LogWarning("Invalid token attempt from {IpAddress}", 
                context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Invalid or expired token\"}");
            return;
        }

        // Store user information in HttpContext for controllers to access
        context.Items["UserId"] = userInfo.Value.UserId;
        context.Items["Username"] = userInfo.Value.Username;
        context.Items["UserRole"] = userInfo.Value.Role;

        _logger.LogInformation("Request authorized successfully for user {Username} with role {Role}", 
            userInfo.Value.Username, userInfo.Value.Role);

        // Token is valid, proceed to the next middleware
        await _next(context);
    }

    private async Task<(Guid UserId, string Username, string Role)?> ValidateTokenWithUserAPI(string token)
    {
        try
        {
            var userApiUrl = Environment.GetEnvironmentVariable("USERAPI_URL") ?? 
                           _configuration["ServiceUrls:UserApi"] ?? 
                           "http://localhost:5160";

            var request = new HttpRequestMessage(HttpMethod.Post, $"{userApiUrl}/api/User/validate-token");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var tokenValidation = System.Text.Json.JsonSerializer.Deserialize<TokenValidationResponse>(content);
            
            return tokenValidation?.IsValid == true ? 
                (tokenValidation.UserId, tokenValidation.Username, tokenValidation.Role) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token with UserAPI");
            return null;
        }
    }
}

/// <summary>
/// Response model for token validation.
/// </summary>
public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
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
