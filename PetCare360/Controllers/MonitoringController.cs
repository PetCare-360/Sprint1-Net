using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare360.DTOs.Responses;
using PetCare360.Services.Interfaces;

namespace PetCare360.Controllers;

[ApiController]
[Route("pets/{id:long}")]
[Authorize]
[Produces("application/json")]
public class MonitoringController(IMonitoringService monitoringService) : ControllerBase
{
    /// <summary>Resumo atual do pet</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(PetSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Summary(long id)
    {
        return Ok(await monitoringService.SummaryAsync(id));
    }

    /// <summary>Histórico de monitoramento</summary>
    [HttpGet("monitoring")]
    [ProducesResponseType(typeof(PagedResponse<SensorDataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Monitoring(
        long id,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        return Ok(await monitoringService.MonitoringAsync(id, page, size));
    }

    /// <summary>Histórico de atividade</summary>
    [HttpGet("activity")]
    [ProducesResponseType(typeof(PagedResponse<SensorDataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activity(
        long id,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        return Ok(await monitoringService.ActivityAsync(id, page, size));
    }

    /// <summary>Última localização do pet</summary>
    [HttpGet("location")]
    [ProducesResponseType(typeof(SensorDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Location(long id)
    {
        var result = await monitoringService.LocationAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Alertas gerados para o pet</summary>
    [HttpGet("alerts")]
    [ProducesResponseType(typeof(PagedResponse<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Alerts(
        long id,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        return Ok(await monitoringService.AlertsAsync(id, page, size));
    }
}