using System.ComponentModel.DataAnnotations;

namespace AiTicketClassifier.Api.Models;

public sealed class TicketAnalyzeRequest
{
    [Required]
    [MinLength(5)]
    public string Message { get; set; } = string.Empty;
}
