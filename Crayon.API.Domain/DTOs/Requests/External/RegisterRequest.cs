using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Requests.Common;

namespace Crayon.API.Domain.DTOs.Requests.External;

public class RegisterRequest
{
    public Guid? CustomerId { get; set; }
    [MinLength(5)]
    [MaxLength(50)]
    public required string Username { get; set; }
    [MinLength(5)]
    [MaxLength(50)]
    public required string Email { get; set; }
    [MinLength(5)]
    [MaxLength(50)]
    public required string Name { get; set; }
    [MinLength(5)]
    [MaxLength(50)]
    public required string Password { get; set; }

    [JsonIgnore]
    public CommonUserContextModel? UserContext { get; set; }
}