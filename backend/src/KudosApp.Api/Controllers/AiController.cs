using KudosApp.Core.DTOs.Ai;
using KudosApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KudosApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiSuggestionService _suggestionService;

    public AiController(IAiSuggestionService suggestionService)
    {
        _suggestionService = suggestionService;
    }

    [HttpPost("suggest-message")]
    public async Task<ActionResult<SuggestMessageResponse>> SuggestMessage(SuggestMessageRequest request)
    {
        var suggestions = await _suggestionService.SuggestMessagesAsync(
            request.CategoryName, request.Intent);

        return Ok(new SuggestMessageResponse(suggestions));
    }
}
