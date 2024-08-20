using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Base;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext ctx) : BaseRepository<CustomerEntity>(ctx), ICustomerRepository
{
    public async Task<bool> CustomerExists(string contactEmail)
    {
        return await _table.AsNoTracking()
            .AnyAsync(x => x.Status == EntityStatus.Active && x.ContactEmail == contactEmail);
    }
}