using System.Text;
using Crayon.API.Domain.Entities.UserRoles;
using Crayon.API.Domain.Entities.Users;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.DbContexts;
using CrayonCCP.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Crayon.API.Infrastructure.Extensions;

public static class DbConfigurationExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment builderEnvironment)
    {
        var dbConnectionString = configuration.GetConnectionString("CrayonDbConnectionString");
        if (string.IsNullOrEmpty(dbConnectionString))
            throw new Exception("CrayonDbConnectionString can't be null or empty!");
        
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

        await InitializeUserRolesAndDefaultUser(dbContext, app.Configuration, logger);
        // await InitializeClients(dbContext, app.Configuration, logger);
    }

    private static async Task InitializeUserRolesAndDefaultUser(AppDbContext context, IConfiguration configuration,
        ILogger<AppDbContext> logger)
    {
        await InitializeRoles(context, configuration);
        await InitializeDefaultUser(context, configuration);
    }
        

    private static async Task InitializeRoles(AppDbContext context, IConfiguration configuration)
    {
        await GetOrInitRole(context, "Admin");
        await GetOrInitRole(context, "User");
    }

    private static async Task GetOrInitRole(AppDbContext context, string name)
    {
        string normalizedName = name.ToUpper();
        var roleExists = await context.UserRoles.AsNoTracking()
            .AnyAsync(x => x.Status == EntityStatus.Active && x.NormalizedName == normalizedName);

        if (roleExists) return;

        var role = new UserRoleEntity()
        {
            Identifier = Guid.NewGuid(),
            Name = name,
            NormalizedName = normalizedName,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AssignedRoles = []
        };
        await context.UserRoles.AddAsync(role);
        await context.SaveChangesAsync();
    }

    private static async Task<UserRoleEntity> GetRoleByNormalizedName(AppDbContext context, string normalizedName)
    {
        return await context.UserRoles.FirstAsync(x =>
            x.Status == EntityStatus.Active && x.NormalizedName == normalizedName);
    }

    private static async Task InitializeDefaultUser(AppDbContext context, IConfiguration configuration)
    {
        var userRole = await GetRoleByNormalizedName(context, "USER");
        var adminRole = await GetRoleByNormalizedName(context, "ADMIN");
        
        var defaultUserSection = configuration.GetSection("Security:InitialUserDetails");
        var defaultUsername = defaultUserSection.GetValue<string>("Username");
        var defaultEmail = defaultUserSection.GetValue<string>("Email");
        var defaultPassword = defaultUserSection.GetValue<string>("Password");
        var usersExist = await context.Users
            .AsNoTracking()
            .Include(x => x.AssignedRoles)
            .ThenInclude(x => x.UserRole)
            .AnyAsync(x => x.Status == EntityStatus.Active);


        if ((string.IsNullOrEmpty(defaultUsername) || string.IsNullOrEmpty(defaultEmail) ||
             string.IsNullOrEmpty(defaultPassword)) && !usersExist)
        {
            throw new Exception("No users were present in the DB, and Initial USer Details aren't configured properly!");
        }

        byte[] bytePasswd = Sha256Extensions.Hash(defaultPassword!);

        string normalizedAdminRole = "ADMIN";
        var existingUser = await context.Users
            .AsNoTracking()
            .Include(x => x.AssignedRoles)
            .ThenInclude(x => x.UserRole)
            .Where(x => x.AssignedRoles
                .Any(y => y.UserRole != null 
                          && y.UserRole.NormalizedName == normalizedAdminRole))
            .FirstOrDefaultAsync(x => x.Status == EntityStatus.Active 
                                      && x.Username == defaultUsername 
                                      && x.Password == bytePasswd);

        if (existingUser is null)
        {
            existingUser = new UserEntity()
            {
                Identifier = Guid.NewGuid(),
                Username = defaultUsername!,
                Email = defaultEmail!,
                Password = bytePasswd,
                Name = "Admin",
                AssignedRoles = [CreateFromRole(userRole), CreateFromRole(adminRole)],
                Status = EntityStatus.Active,
                Verified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();
        }
    }

    private static AssignedUserRoleEntity CreateFromRole(UserRoleEntity userRole)
    {
        return new()
        {
            Identifier = Guid.NewGuid(),
            UserRole = userRole,
            Status = EntityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}