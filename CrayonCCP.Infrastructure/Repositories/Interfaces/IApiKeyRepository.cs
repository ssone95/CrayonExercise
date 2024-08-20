using CrayonCCP.Domain.Entities.Clients;

namespace CrayonCCP.Infrastructure.Repositories.Interfaces;

public interface IApiKeyRepository : IBaseRepository<ApiKeyEntity>
{
    Task<ApiKeyEntity?> FindByHashValueAsync(byte[] hashValue);
}