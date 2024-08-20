using Crayon.API.Domain.Entities.Base;
using Crayon.API.Domain.Entities.Users;

namespace Crayon.API.Domain.Entities.UserRoles;

public class UserRoleEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string NormalizedName { get; set; }
    
    public virtual required List<AssignedUserRoleEntity> AssignedRoles { get; set; }
}