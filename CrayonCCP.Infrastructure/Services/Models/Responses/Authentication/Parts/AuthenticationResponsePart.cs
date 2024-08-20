using System.Security.Claims;
using CrayonCCP.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace CrayonCCP.Infrastructure.Services.Models.Responses.Authentication.Parts;

public class AuthenticationResponsePart
{
    public required AuthenticationResultEnum Result { get; init; }
    
    public ClaimsIdentity? Client { get; init; }
}