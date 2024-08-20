using System.ComponentModel.DataAnnotations;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Orders;

namespace Crayon.API.Domain.Entities.Customers;

public class CustomerEntity : BaseEntity
{
    [MinLength(5)]
    [MaxLength(50)]
    public required string CustomerName { get; set; }
    
    [MinLength(5)]
    [MaxLength(50)]
    public required string ContactEmail { get; set; }
    
    public bool IsServiceroker { get; set; }
    
    public virtual required List<OrderEntity> CustomerOrders { get; set; }
    
    public virtual required List<CustomerAccountEntity> CustomerAccounts { get; set; }
}