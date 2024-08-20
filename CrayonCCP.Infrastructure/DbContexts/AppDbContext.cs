using CrayonCCP.Domain.Entities.Clients;
using CrayonCCP.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace CrayonCCP.Infrastructure.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BaseServiceEntity> ServiceEntities { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<ApiKeyEntity> ApiKeys { get; set; }
}