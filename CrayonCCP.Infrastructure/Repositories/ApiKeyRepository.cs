using CrayonCCP.Domain.Entities.Clients;
using CrayonCCP.Domain.Enums;
using CrayonCCP.Infrastructure.DbContexts;
using CrayonCCP.Infrastructure.Repositories.Base;
using CrayonCCP.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrayonCCP.Infrastructure.Repositories;

public class ApiKeyRepository(AppDbContext ctx) : BaseRepository<ApiKeyEntity>(ctx), IApiKeyRepository
{
    public async Task<ApiKeyEntity?> FindByHashValueAsync(byte[] hashValue)
    {
        return await _table.FirstOrDefaultAsync(x => x.ApiKeyHash == hashValue && x.Status == EntityStatus.Active);
    }
}