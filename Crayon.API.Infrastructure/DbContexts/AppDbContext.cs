using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Licenses;
using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Domain.Entities.UserRoles;
using Crayon.API.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<AssignedUserRoleEntity> AssignedRoles { get; set; }
    
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<CustomerAccountEntity> CustomerAccounts { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderLineEntity> OrderLines { get; set; }
    public DbSet<LicenseEntity> Licenses { get; set; }
}