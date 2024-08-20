using CrayonCCP.Domain.Entities.Base;
using CrayonCCP.Infrastructure.DbContexts;
using CrayonCCP.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrayonCCP.Infrastructure.Repositories.Base;

public abstract class BaseRepository<T>(AppDbContext ctx) : IBaseRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _dbContext = ctx;
    protected readonly DbSet<T> _table = ctx.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _table.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T?> GetByIdentifierAsync(Guid identifier)
    {
        return await _table.AsNoTracking().SingleOrDefaultAsync(x => x.Identifier == identifier);
    }

    public virtual async Task<List<T>> GetByAsync(Func<T, bool> filter)
    {
        var query = _table.Where(filter).AsQueryable();
        return await query.ToListAsync();
    }

    public virtual async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}