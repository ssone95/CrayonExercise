using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Base;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories;

public class OrderRepository(AppDbContext ctx) : BaseRepository<OrderEntity>(ctx), IOrderRepository
{
    public new async Task<OrderEntity?> GetByIdentifierAsync(Guid identifier)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.CustomerAccount)
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Identifier == identifier);
    }
    
    public async Task<(List<OrderEntity> items, int totalPages)> GetByAccountIdentifierAsync(Guid accountIdentifier, int currentPage, int itemsPerPage)
    {
        var totalCount = await _table.AsNoTracking()
            .Include(x => x.CustomerAccount)
            .Where(x => x.CustomerAccount != null)
            .CountAsync(x => x.CustomerAccount!.Identifier == accountIdentifier);

        currentPage = Math.Max(currentPage, 1);
        itemsPerPage = Math.Max(itemsPerPage, 1);
        var itemsToSkip = (currentPage - 1) * itemsPerPage;

        var totalPages = Convert.ToInt32(Math.Ceiling(totalCount / (itemsPerPage * 1.0)));
        var pagedItems = await _table
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.CustomerAccount)
            .Include(x => x.Lines)
            .Where(x => x.CustomerAccount != null && x.CustomerAccount.Identifier == accountIdentifier)
            .Skip(itemsToSkip)
            .Take(itemsPerPage)
            .ToListAsync();
        return (pagedItems, totalPages);
    }
}