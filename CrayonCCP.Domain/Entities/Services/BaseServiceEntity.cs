using System.ComponentModel.DataAnnotations;
using CrayonCCP.Domain.Entities.Base;

namespace CrayonCCP.Domain.Entities.Services;

public class BaseServiceEntity : BaseEntity
{
    [MinLength(5)]
    [MaxLength(50)]
    public required string Name { get; set; }
    
    public double Price { get; set; }
}