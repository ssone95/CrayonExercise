using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Users;

namespace Crayon.API.Infrastructure.Repositories.Interfaces;

public interface IUserAuthenticationRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> AuthenticateBasic(string username, byte[] password);
    Task<UserEntity?> AuthenticateJwt(string email, Guid userId);
    Task<CustomerAccountEntity?> AuthenticateCustomerBasic(string username, byte[] password);
}