using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Domain.Entities.Orders;

public class OrderLineEntity : BaseEntity
{
    public int? OrderId { get; set; }
    [ForeignKey("OrderId")]
    public OrderEntity? Order { get; set; }
    
    public Guid ServiceId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TaxPerUnitMultiplier { get; set; }
    public double BaseTotalPrice { get; set; }
    public double TotalTax { get; set; }
    public double TotalPrice { get; set; }
    
    [MinLength(5)]
    [MaxLength(50)]
    public required string ServiceName { get; set; }
    
    public EntityStatus LicenseStatus { get; set; }
    
    public DateTime ValidUntil { get; set; }
}