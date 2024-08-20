using CrayonCCP.Domain.Entities.Clients;
using CrayonCCP.Infrastructure.DbContexts;
using CrayonCCP.Infrastructure.Repositories.Base;
using CrayonCCP.Infrastructure.Repositories.Interfaces;

namespace CrayonCCP.Infrastructure.Repositories;

public class ClientRepository(AppDbContext ctx) : BaseRepository<ClientEntity>(ctx), IClientRepository
{
    
}