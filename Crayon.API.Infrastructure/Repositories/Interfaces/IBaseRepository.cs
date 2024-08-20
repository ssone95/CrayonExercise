using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdentifierAsync(Guid identifier);
    Task<List<T>> GetByAsync(Func<T, bool> filter);
    Task SaveChangesAsync();
    Task<T> Create(T entity);
    Task<T> Update(T entity);
    Task<T> SetStatus(T entity, EntityStatus status);
}