using Crayon.API.Domain.Entities.Customers;

namespace Crayon.API.Infrastructure.Repositories.Interfaces;

public interface ICustomerRepository : IBaseRepository<CustomerEntity>
{
    public Task<bool> CustomerExists(string contactEmail);
}