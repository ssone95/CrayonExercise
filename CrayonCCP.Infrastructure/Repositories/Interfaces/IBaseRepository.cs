using CrayonCCP.Domain.Entities.Base;

namespace CrayonCCP.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdentifierAsync(Guid identifier);
    Task<List<T>> GetByAsync(Func<T, bool> filter);
    Task SaveChangesAsync();
}