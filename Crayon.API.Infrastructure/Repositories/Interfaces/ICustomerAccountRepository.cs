using Crayon.API.Domain.Entities.Customers;

namespace Crayon.API.Infrastructure.Repositories.Interfaces;

public interface ICustomerAccountRepository : IBaseRepository<CustomerAccountEntity>
{
    Task<bool> AccountExists(string username, string email);
    Task<(List<CustomerAccountEntity>, int totalPages)> GetAccountsByCustomerIdentifier(Guid customerId, string? filterString, int currentPage, int itemsPerPage);
}