namespace CrayonCCP.Infrastructure.Services.Interfaces;

public interface ICacheService
{
    Task<string?> GetString(string key);
    Task<string?> SetString(string key, string? value, TimeSpan expiration);
    Task<string?> PopString(string key);
    Task DeleteString(string key);
    Task<bool> StringExists(string key);
}