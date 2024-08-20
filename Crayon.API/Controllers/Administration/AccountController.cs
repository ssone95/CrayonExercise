using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Internal.UserAuthentication;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Controllers.Administration;

[Authorize(Roles = "ADMIN,USER")]
[ApiController]
[Route("administration/[controller]/[action]")]
public class AccountController(IAuthenticatorService authenticatorService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register()
    {
        await Task.Delay(300);
        throw new NotImplementedException();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var authenticationResponse = await authenticatorService.Login(new AuthenticateUserBasicRequest()
        {
            LoginRequest = request,
            IsCustomer = false
        });
        return authenticationResponse.Success ? Ok(authenticationResponse) : BadRequest(authenticationResponse);  
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        await Task.Delay(300);
        throw new NotImplementedException();
    }
}