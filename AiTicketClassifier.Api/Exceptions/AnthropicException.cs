namespace AiTicketClassifier.Api.Exceptions;

public class AnthropicException(int statusCode, string responseText)
    : Exception($"Anthropic API request failed with status code: {statusCode}. Response: {responseText}")
{
}
