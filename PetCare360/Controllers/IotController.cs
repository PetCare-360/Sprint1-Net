using Microsoft.AspNetCore.Mvc;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Services.Interfaces;

namespace PetCare360.Controllers;

[ApiController]
[Route("api/iot")]
[Produces("application/json")]
public class IotController(IIotProcessingService iotService) : ControllerBase
{
    /// <summary>Receber dados da coleira inteligente</summary>
    [HttpPost("data")]
    [ProducesResponseType(typeof(IotDataResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Receive([FromBody] IotDataRequest request)
    {
        var result = await iotService.ProcessAsync(request);
        return Created(string.Empty, result);
    }
}