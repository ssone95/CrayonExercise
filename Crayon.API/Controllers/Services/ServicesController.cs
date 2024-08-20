using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crayon.API.Controllers.Services;

[Authorize(Roles = "ADMIN,USER,MANAGER")]
[ApiController]
[Route("[controller]/[action]")]
public class ServicesController(ICCPDataService ccpDataService) : Controller
{
    [HttpPost]
    [Consumes(typeof(ListServicesRequest), "application/json")]
    [ProducesResponseType(typeof(ListServicesResponse), 200, "application/json")]
    [ProducesResponseType(typeof(ListServicesResponse), 400, "application/json")]
    [ProducesResponseType(typeof(object), 400, "application/json")]
    public async Task<IActionResult> List([FromBody] ListServicesRequest request)
    {
        ListServicesResponse listResult = await ccpDataService.GetAvailableServices(request);
        return listResult.Success
            ? Ok(listResult)
            : BadRequest(listResult);
    }
}