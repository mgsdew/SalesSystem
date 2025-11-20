using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserAPI.Models.DTOs;
using UserAPI.Models.Entities;

namespace UserAPI.Services;

/// <summary>
/// Service for generating and validating secure tokens.
/// </summary>
public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;

        // Get encryption key from environment or generate one
        var keyString = Environment.GetEnvironmentVariable("TOKEN_ENCRYPTION_KEY") ??
                       _configuration["TokenSettings:EncryptionKey"] ??
                       "DefaultKeyForDevelopmentOnly1234567890123456";

        _key = Encoding.UTF8.GetBytes(keyString.PadRight(32, '0').Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes("DefaultIV12345678".PadRight(16, '0').Substring(0, 16));
    }

    /// <summary>
    /// Generates a secure token for an authenticated user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>A secure token containing user information.</returns>
    public string GenerateToken(User user)
    {
        var tokenData = $"{user.Id}:{user.Username}:{user.Role}:{DateTime.UtcNow.Ticks}";
        return Encrypt(tokenData);
    }

    /// <summary>
    /// Validates a token and extracts user information.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>User information if valid, null if invalid or expired.</returns>
    public (Guid UserId, string Username, UserRole Role)? ValidateToken(string token)
    {
        try
        {
            var decryptedData = Decrypt(token);
            var parts = decryptedData.Split(':');

            if (parts.Length != 4)
                return null;

            if (!Guid.TryParse(parts[0], out var userId))
                return null;

            if (!Enum.TryParse<UserRole>(parts[2], out var role))
                return null;

            // Check if token is not older than 24 hours
            if (!long.TryParse(parts[3], out var ticks))
                return null;

            var tokenTime = new DateTime(ticks);
            if (DateTime.UtcNow - tokenTime > TimeSpan.FromHours(24))
                return null;

            return (userId, parts[1], role);
        }
        catch
        {
            return null;
        }
    }

    private string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(plainText);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    private string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}