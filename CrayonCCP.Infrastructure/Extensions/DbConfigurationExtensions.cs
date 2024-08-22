using CrayonCCP.Domain.Entities.Clients;
using CrayonCCP.Domain.Enums;
using CrayonCCP.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CrayonCCP.Infrastructure.Extensions;

public static class DbConfigurationExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment builderEnvironment)
    {
        var dbConnectionString = configuration.GetConnectionString("CCPDbConnectionString");
        if (string.IsNullOrEmpty(dbConnectionString))
            throw new Exception("CCPDbConnectionString can't be null or empty!");
        
        Console.WriteLine($"Connection string used: {dbConnectionString}");
        return services.AddDbContext<AppDbContext>(o =>
        {
            o.UseSqlServer(dbConnectionString, dbo =>
            {
                dbo.CommandTimeout(builderEnvironment.IsDevelopment() ? 90 : 60);
            });
        });
    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying Db Migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Applying Db Migrations - done!");
        }
        else
            logger.LogInformation("No pending migrations!");

        await InitializeClients(dbContext, app.Configuration, logger);
    }

    private static async Task InitializeClients(AppDbContext dbContext, IConfiguration configuration,
        ILogger<AppDbContext> logger)
    {
        var crayonSection = configuration.GetSection("InitialClients:Crayon");
        var crayonClientId = crayonSection.GetValue("Id", string.Empty);
        var crayonName = crayonSection.GetValue("Name", string.Empty);
        var crayonSecret = crayonSection.GetValue("Secret", string.Empty);

        if (string.IsNullOrEmpty(crayonClientId) || string.IsNullOrEmpty(crayonSecret) 
            || string.IsNullOrEmpty(crayonName) || !Guid.TryParse(crayonClientId, out Guid crayonClientIdentifier))
        {
            logger.LogWarning("An error occurred while trying to retrieve and parse initial client's data (Crayon)!\nId: {crayonClientId}, Name: {crayonName}, Secret: {crayonSecret}",
                crayonClientId, crayonName, crayonSecret);
            return;
        }

        var crayonClient = await dbContext.Clients.SingleOrDefaultAsync(x => x.Identifier == crayonClientIdentifier
                                                                             && x.Status == EntityStatus.Active);
        if (crayonClient is null)
        {
            crayonClient = new ClientEntity()
            {
                Identifier = crayonClientIdentifier,
                Name = crayonName!,
                Description = null,

                Status = EntityStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ApiKeys = []
            };
            
            await dbContext.Clients.AddAsync(crayonClient);
            await dbContext.SaveChangesAsync();
        }
        
        var apiKeyHash = Sha256Extensions.Hash(crayonSecret);
        var existingApiKey = await dbContext.ApiKeys.SingleOrDefaultAsync(x => x.ClientId == crayonClient.Id &&
                                                                               x.Status == EntityStatus.Active);
        if (existingApiKey is null)
        {
            await AddApiKey(dbContext, apiKeyHash, crayonClient);
        }
        else
        {
            if (existingApiKey.ExpiresOn <= DateTime.UtcNow)
            {
                existingApiKey.Status = EntityStatus.Inactive;
                await AddApiKey(dbContext, apiKeyHash, crayonClient);
            }
        }
    }

    private static async Task AddApiKey(AppDbContext dbContext, byte[] apiKeyHash, ClientEntity crayonClient)
    {
        var apiKey = new ApiKeyEntity()
        {
            Identifier = Guid.NewGuid(),
            ApiKeyHash = apiKeyHash,
            ClientId = crayonClient.Id,
            ExpiresOn = DateTime.UtcNow.AddDays(90),

            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();
    }
}