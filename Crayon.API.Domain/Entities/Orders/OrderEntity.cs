using System.ComponentModel.DataAnnotations.Schema;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Domain.Entities.Orders;

public class OrderEntity : BaseEntity
{
    public double TransactionFee { get; set; }
    public double Total { get; set; }
    public double TotalTax { get; set; }
    public double GrandTotal { get; set; }
    
    public OrderStatus OrderStatus { get; set; }
    
    public int? CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public CustomerEntity? Customer { get; set; }
    
    public int? CustomerAccountId { get; set; }
    [ForeignKey("CustomerAccountId")]
    public CustomerAccountEntity? CustomerAccount { get; set; }
    
    public virtual required List<OrderLineEntity> Lines { get; set; }
}