using System.ComponentModel.DataAnnotations.Schema;
using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.UserRoles;

namespace Crayon.API.Domain.Entities.Users;

public class AssignedUserRoleEntity : BaseEntity
{
    public int? UserId { get; set; }
    [ForeignKey("UserId")]
    public UserEntity? User { get; set; }
    
    public int? UserRoleId { get; set; }
    [ForeignKey("UserRoleId")]
    public UserRoleEntity? UserRole { get; set; }
}