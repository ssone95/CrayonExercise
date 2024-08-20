using CrayonCCP.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CrayonCCP.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly ILogger<RedisCacheService> _logger;
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly IDatabase _database;

    public RedisCacheService(IConfiguration configuration, IHostEnvironment environment, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        
        var redisConnString = configuration.GetConnectionString("CCPRedisConnectionString");
        if (string.IsNullOrEmpty(redisConnString))
            throw new ArgumentNullException(nameof(redisConnString), "Redis connection string must be provided!");
            
        _multiplexer = ConnectionMultiplexer.Connect(redisConnString, o =>
        {
            o.IncludeDetailInExceptions = environment.IsDevelopment();
            o.Ssl = false;
            o.AllowAdmin = true;
            o.CheckCertificateRevocation = false;
        });

        _database = _multiplexer.GetDatabase();
    }

    public async Task<string?> GetString(string key)
    {
        if (!(await StringExists(key))) return RedisValue.Null;

        var result = await _database.StringGetAsync(key, CommandFlags.PreferMaster);

        if (!result.HasValue || result.IsNullOrEmpty)
        {
            return null;
        }

        return result;
    }

    public async Task<string?> SetString(string key, string? value, TimeSpan expiration)
    {
        return await _database.StringSetAndGetAsync(key, value, expiration, When.Always, CommandFlags.DemandMaster);
    }

    public async Task<string?> PopString(string key)
    {
        if (!(await StringExists(key))) return RedisValue.Null;

        return await _database.StringGetDeleteAsync(key, CommandFlags.PreferMaster);
    }

    public async Task DeleteString(string key)
    {
        await PopString(key);
    }

    public async Task<bool> StringExists(string key)
    {
        return await _database.KeyExistsAsync(key, CommandFlags.PreferMaster);
    }
}