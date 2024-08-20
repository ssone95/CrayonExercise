using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Users;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Base;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories;

public class UserAuthenticationRepository(AppDbContext ctx) : BaseRepository<UserEntity>(ctx), IUserAuthenticationRepository
{
    public async Task<UserEntity?> AuthenticateBasic(string username, byte[] password)
    {
        var users = await _table.AsNoTracking()
            .Include(x => x.AssignedRoles)
                .ThenInclude(x => x.UserRole)
            .Where(x 
                => x.Status == EntityStatus.Active && x.Verified 
               && x.Username == username && x.Password == password)
            .ToListAsync();
        return users.SingleOrDefault();   
    }

    public async Task<CustomerAccountEntity?> AuthenticateCustomerBasic(string username, byte[] password)
    {
        var user = await _dbContext.CustomerAccounts
            .AsNoTracking()
            .Include(x => x.Customer)
            .Where(x => x.Verified
                                              && x.Status == EntityStatus.Active && x.Username == username &&
                                              x.Password == password)
            .SingleOrDefaultAsync();
        return user;
    }

    public async Task<UserEntity?> AuthenticateJwt(string email, Guid userId)
    {
        var users = await GetByAsync(x 
            => x is { Status: EntityStatus.Active, Verified: true } 
               && x.Email == email && x.Identifier == userId);
        return users.SingleOrDefault();
    }
}