using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Users;
using Crayon.API.Domain.Enums;
using Microsoft.IdentityModel.Tokens;

namespace Crayon.API.Infrastructure.Extensions;

public static class JwtHelpers
{
    public static (string token, DateTime expiresOn) GenerateToken(CustomerAccountEntity customer, TimeSpan expiresAfter, string secret, string issuer, string audience)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Sid, customer.Identifier.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Upn, customer.Email),
            new Claim(ClaimTypes.Name, customer.Name),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, "SUBCUSTOMER")
        ];

        claims.AddRange(GetRolesByCustomerType(customer));
        
        var expirationDate = DateTime.UtcNow.AddMinutes(expiresAfter.TotalMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expirationDate);
    }

    private static List<Claim> GetRolesByCustomerType(CustomerAccountEntity accountEntity)
    {
        if (accountEntity.Customer is null)
            return [];
        
        return accountEntity switch
        {
            { IsManagementAccount: true, Customer.IsServiceroker: true } =>
            [
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "MANAGER"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "CUSTOMER"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "BROKER"),
                new Claim(ClaimTypes.Spn, accountEntity.Customer.Identifier.ToString())
            ],
            { IsManagementAccount: true, Customer.IsServiceroker: false } =>
            [
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "MANAGER"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "CUSTOMER"),
                new Claim(ClaimTypes.Spn, accountEntity.Customer.Identifier.ToString())
            ],
            { IsManagementAccount: false, Customer.IsServiceroker: true } => 
            [
                new Claim(ClaimTypes.Spn, accountEntity.Customer.Identifier.ToString())
            ],
            _ => [
                new Claim(ClaimTypes.Spn, accountEntity.Customer.Identifier.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "CUSTOMER")
            ]
        };
    }
    
    public static (string token, DateTime expiresOn) GenerateToken(UserEntity user, TimeSpan expiresAfter, string secret, string issuer, string audience)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var roleClaims = user.AssignedRoles.Where(x
                => x.UserRole is not null && x.Status == EntityStatus.Active)
            .Select(x => x.UserRole!.NormalizedName)
            .Select(x => new Claim(ClaimsIdentity.DefaultRoleClaimType, x));
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Sid, user.Identifier.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Upn, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        ];
        claims.AddRange(roleClaims);

        var expirationDate = DateTime.UtcNow.AddMinutes(expiresAfter.TotalMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expirationDate);
    }
}