using System.Security.Claims;
using KudosApp.Core.DTOs.Kudos;
using KudosApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KudosApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KudosController : ControllerBase
{
    private readonly IKudosService _kudosService;

    public KudosController(IKudosService kudosService)
    {
        _kudosService = kudosService;
    }

    [HttpPost]
    public async Task<ActionResult<KudosResponse>> Create(CreateKudosRequest request)
    {
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var result = await _kudosService.CreateAsync(senderId, request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<KudosFeedResponse>> GetFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 50) pageSize = 20;

        var result = await _kudosService.GetFeedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<KudosResponse>> GetById(int id)
    {
        var result = await _kudosService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("sent")]
    public async Task<ActionResult<IReadOnlyList<KudosResponse>>> GetSent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _kudosService.GetBySenderAsync(userId);
        return Ok(result);
    }

    [HttpGet("received")]
    public async Task<ActionResult<IReadOnlyList<KudosResponse>>> GetReceived()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _kudosService.GetByReceiverAsync(userId);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _kudosService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
