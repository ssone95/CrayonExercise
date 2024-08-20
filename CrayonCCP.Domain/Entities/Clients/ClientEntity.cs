using System.ComponentModel.DataAnnotations;
using CrayonCCP.Domain.Entities.Base;

namespace CrayonCCP.Domain.Entities.Clients;

public class ClientEntity : BaseEntity
{
    [MinLength(5)]
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [MaxLength(512)]
    public string? Description { get; set; }
    
    public virtual required List<ApiKeyEntity> ApiKeys { get; set; }
}