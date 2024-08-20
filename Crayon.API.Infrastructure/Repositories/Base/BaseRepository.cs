using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.DbContexts;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Crayon.API.Infrastructure.Repositories.Base;

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

    public virtual async Task<T> Create(T entity)
    {
        if (entity.Identifier == Guid.Empty)
            entity.Identifier = Guid.NewGuid();
        entity.Status = EntityStatus.Active;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _table.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _table.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        return await Task.FromResult(entity);
    }

    public virtual async Task<T> SetStatus(T entity, EntityStatus status)
    {
        entity.Status = status;
        return await Update(entity);
    }

    public virtual async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    protected IQueryable<TQuery> PaginateQuery<TQuery>(int currentPage, int itemsPerPage, IQueryable<TQuery> query)
    {
        return query
            .Skip((currentPage - 1) * itemsPerPage)
            .Take(itemsPerPage);
    }

    protected (int currentPage, int itemsPerPage) FixPaginationNumbers(int currentPage, int itemsPerPage)
    {
        if (currentPage < 1) currentPage = 1;
        if (itemsPerPage < 1) itemsPerPage = 1;
        return (currentPage, itemsPerPage);
    }
}