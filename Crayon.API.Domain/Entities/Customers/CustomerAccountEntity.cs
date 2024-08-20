using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Orders;

namespace Crayon.API.Domain.Entities.Customers;

public class CustomerAccountEntity : BaseEntity
{
    public int? CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public CustomerEntity? Customer { get; set; }
    
    [MinLength(5)]
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [MinLength(5)]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [MinLength(5)]
    [MaxLength(50)]
    public required string Email { get; set; }
    
    [MinLength(32)]
    [MaxLength(32)]
    public required byte[] Password { get; set; }
    
    public bool IsManagementAccount { get; set; } // This would be swapped with roles if implemented correctly
    
    public bool Verified { get; set; }
    
    public virtual required List<OrderEntity> AccountOrders { get; set; }
}