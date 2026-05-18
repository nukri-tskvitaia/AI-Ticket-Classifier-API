using AiTicketClassifier.Api.Models;
using AiTicketClassifier.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiTicketClassifier.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController(ClaudeTicketAnalyzer analyzer) : ControllerBase
{
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(TicketAnalyzeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketAnalyzeResponse>> Analyze([FromBody] TicketAnalyzeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await analyzer.AnalyzeAsync(request.Message, cancellationToken);

        return Ok(result);
    }
}
