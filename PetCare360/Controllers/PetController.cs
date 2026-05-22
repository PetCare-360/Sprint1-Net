using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Services.Interfaces;

namespace PetCare360.Controllers;

[ApiController]
[Route("pets")]
[Authorize]
[Produces("application/json")]
public class PetController(IPetService petService) : ControllerBase
{
    /// <summary>Listar pets com paginação</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PetPageResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 0,
        [FromQuery] int size = 10)
    {
        return Ok(await petService.ListAsync(page, size));
    }

    /// <summary>Listar todos os pets sem paginação</summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<PetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAll()
    {
        return Ok(await petService.ListAllAsync());
    }

    /// <summary>Cadastrar pet com primeira leitura de sensores</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] PetRequest request)
    {
        var result = await petService.CreateAsync(request);
        return CreatedAtAction(nameof(Find), new { id = result.Id }, result);
    }

    /// <summary>Buscar pet por ID</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Find(long id)
    {
        return Ok(await petService.FindAsync(id));
    }

    /// <summary>Status consolidado de saúde do pet</summary>
    [HttpGet("{id:long}/health-status")]
    [ProducesResponseType(typeof(PetHealthStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HealthStatus(long id)
    {
        return Ok(await petService.HealthStatusAsync(id));
    }

    /// <summary>Pets em alerta ou estado crítico</summary>
    [HttpGet("quick-alerts")]
    [ProducesResponseType(typeof(IEnumerable<QuickAlertPetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QuickAlerts()
    {
        return Ok(await petService.QuickAlertsAsync());
    }

    /// <summary>Resumo de atividade das últimas 24h</summary>
    [HttpGet("{id:long}/activity-summary")]
    [ProducesResponseType(typeof(ActivitySummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivitySummary(long id)
    {
        return Ok(await petService.ActivitySummaryAsync(id));
    }

    /// <summary>Atualizar pet</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] PetRequest request)
    {
        return Ok(await petService.UpdateAsync(id, request));
    }

    /// <summary>Remover pet</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        await petService.DeleteAsync(id);
        return NoContent();
    }
}