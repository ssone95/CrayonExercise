using System.ComponentModel.DataAnnotations;
using Crayon.API.Domain.Entities.Base;

namespace Crayon.API.Domain.Entities.Users;

public class UserEntity : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    
    public bool Verified { get; set; }
    
    [MinLength(32)]
    [MaxLength(32)]
    public required byte[] Password { get; set; }
    
    public virtual required List<AssignedUserRoleEntity> AssignedRoles { get; set; }
}