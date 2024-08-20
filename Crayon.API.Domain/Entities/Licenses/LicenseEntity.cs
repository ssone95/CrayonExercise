using System.ComponentModel.DataAnnotations.Schema;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Domain.Entities.Licenses;

public class LicenseEntity : BaseEntity
{
    public int? OrderLineId { get; set; }
    [ForeignKey("OrderLineId")]
    public OrderLineEntity? OrderLine { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
    public LicenseStatus LicenseStatus { get; set; }
}