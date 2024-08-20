using CrayonCCP.Domain.Enums;

namespace CrayonCCP.Domain.Entities.Base;

public class BaseEntity
{
    public int Id { get; set; }
    public Guid Identifier { get; set; }

    public EntityStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}