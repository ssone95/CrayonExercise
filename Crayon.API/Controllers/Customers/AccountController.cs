using System.Net;
using System.Security.Authentication;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Requests.Internal.UserAuthentication;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;
using Crayon.API.Infrastructure.Extensions;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Controllers.Customers;

[Authorize(Roles = "CUSTOMER,MANAGER,SUBCUSTOMER,BROKER")]
[ApiController]
[Route("customers/[controller]/[action]")]
public class AccountController(IAuthenticatorService authenticatorService, IRegistrationService registrationService,
    ICustomerDataService customerDataService) : ControllerBase
{
    
    [AllowAnonymous]
    [HttpPost]
    [Consumes(typeof(RegisterRequest), "application/json")]
    [ProducesResponseType(typeof(CustomerRegistrationResponse), 200, "application/json")]
    [ProducesResponseType(typeof(CustomerRegistrationResponse), 400, "application/json")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        CustomerRegistrationResponse registrationResponse = await registrationService.Register(new()
        {
            RegistrationRequest = request,
            IsServiceBroker = false,
            SubAccountRegistration = false
        });

        return registrationResponse.Success 
            ? Ok(registrationResponse) 
            : BadRequest(registrationResponse);
    }
    
    [Authorize(Roles = "BROKER")]
    [HttpPost]
    [Consumes(typeof(RegisterRequest), "application/json")]
    [ProducesResponseType(typeof(CustomerRegistrationResponse), 200, "application/json")]
    [ProducesResponseType(typeof(CustomerRegistrationResponse), 400, "application/json")]
    public async Task<IActionResult> RegisterSubAccount([FromBody] RegisterRequest request)
    {
        CustomerAccountDetailsContextModel userContext = HttpContext.GetUserCtx<CustomerAccountDetailsContextModel>();
        if (!userContext.IsAuthenticated) throw new AuthenticationException("Temporary error, please check your credentials and re-login!");

        CustomerRegistrationResponse registrationResponse = await registrationService.Register(new()
        {
            UserContext = userContext,
            RegistrationRequest = request,
            IsServiceBroker = true,
            SubAccountRegistration = true
        });

        return registrationResponse.Success 
            ? Ok(registrationResponse) 
            : BadRequest(registrationResponse);
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Consumes(typeof(LoginRequest), "application/json")]
    [ProducesResponseType(typeof(LoginResponse), 200, "application/json")]
    [ProducesResponseType(typeof(LoginResponse), 400, "application/json")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginResponse authenticationResponse = await authenticatorService.Login(new AuthenticateUserBasicRequest()
        {
            LoginRequest = request,
            IsCustomer = true
        });
        return authenticationResponse.Success 
            ? Ok(authenticationResponse) 
            : BadRequest(authenticationResponse);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        await Task.Delay(300);
        throw new NotImplementedException();
    }

    [Authorize(Roles = "MANAGER")]
    [HttpPost]
    [Consumes(typeof(CustomerAccountListRequest), "application/json")]
    [ProducesResponseType(typeof(CustomerAccountListResponse), 200, "application/json")]
    [ProducesResponseType(typeof(CustomerAccountListResponse), 400, "application/json")]
    [ProducesResponseType(typeof(object), 400, "application/json")]
    public async Task<IActionResult> GetAccounts([FromBody] CustomerAccountListRequest request)
    {
        var userContext = HttpContext.GetUserCtx<CustomerAccountDetailsContextModel>();
        if (!userContext.IsAuthenticated) throw new AuthenticationException("Temporary error, please check your credentials and re-login!");

        if (!userContext.IsManager || !userContext.IsBroker) throw new UnauthorizedAccessException("You don't have access to this resource!");

        request.UserContext = userContext;

        CustomerAccountListResponse response = await customerDataService.GetCustomerAccounts(request);

        return response.Success
            ? Ok(response)
            : BadRequest(response);
    }
}