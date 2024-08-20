using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Base;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories;

public class OrderLineRepository(AppDbContext ctx) : BaseRepository<OrderLineEntity>(ctx), IOrderLineRepository
{
}