using Crayon.API.Domain.Entities.Orders;

namespace Crayon.API.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository : IBaseRepository<OrderEntity>
{
    Task<(List<OrderEntity> items, int totalPages)> GetByAccountIdentifierAsync(Guid accountIdentifier, int currentPage, int itemsPerPage);
}