using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Base;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories;

public class CustomerAccountRepository(AppDbContext ctx) : BaseRepository<CustomerAccountEntity>(ctx), ICustomerAccountRepository
{
    public override async Task<CustomerAccountEntity?> GetByIdentifierAsync(Guid identifier)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Customer)
            .Where(x => x.Status == EntityStatus.Active && x.Customer != null)
            .SingleOrDefaultAsync(x => x.Identifier == identifier);
    }

    public async Task<bool> AccountExists(string username, string email)
    {
        return await _table
            .AsNoTracking()
            .AnyAsync(x => x.Status == EntityStatus.Active && x.Username == username || x.Email == email);
    }

    public async Task<(List<CustomerAccountEntity>, int totalPages)> GetAccountsByCustomerIdentifier(Guid customerId, string? filterString, int currentPage, int itemsPerPage)
    {
        (currentPage, itemsPerPage) = FixPaginationNumbers(currentPage, itemsPerPage);
        
        IQueryable<CustomerAccountEntity> query = _table
            .AsNoTracking()
            .Include(x => x.Customer)
            .Where(x => x.Status == EntityStatus.Active && x.Customer != null && x.Customer.Identifier == customerId)
            .Where(x => string.IsNullOrEmpty(filterString) 
                || x.Email.StartsWith(filterString)
                || x.Name.StartsWith(filterString));
        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(totalCount / itemsPerPage, 1);
        
        query = PaginateQuery(currentPage, itemsPerPage, query);
        List<CustomerAccountEntity> paginatedItems = await query.ToListAsync();
        return (paginatedItems, totalPages);
    }
}