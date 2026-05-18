namespace AiTicketClassifier.Api.Models;

public sealed class TicketAnalyzeResponse
{
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string SuggestedReply { get; set; } = string.Empty;
}
