using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CrayonCCP.Domain.Entities.Base;

namespace CrayonCCP.Domain.Entities.Clients;

public class ApiKeyEntity : BaseEntity
{
    public int? ClientId { get; set; }
    
    [ForeignKey(nameof(ClientId))]
    public ClientEntity? Client { get; set; }
    
    public DateTime? ExpiresOn { get; set; }
    
    [MinLength(32)]
    [MaxLength(32)]
    public required byte[] ApiKeyHash { get; set; }
}