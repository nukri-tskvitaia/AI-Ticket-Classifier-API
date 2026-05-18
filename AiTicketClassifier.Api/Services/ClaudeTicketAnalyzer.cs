using AiTicketClassifier.Api.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiTicketClassifier.Api.Exceptions;
using AiTicketClassifier.Api.Options;
using Microsoft.Extensions.Options;
using AiTicketClassifier.Api.Validation;

namespace AiTicketClassifier.Api.Services;

public sealed class ClaudeTicketAnalyzer(HttpClient httpClient, IOptions<ClaudeOptions> options)
{
    private readonly ClaudeOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<TicketAnalyzeResponse> AnalyzeAsync(string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Ticket message cannot be empty.", nameof(message));

        var apiKey = _options.ApiKey;

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Anthropic API key is missing.");

        var requestBody = CreateRequestBody(message, _options);

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.MessagesEndpointUrl);
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", _options.Version);

        var mediaType = MediaTypeHeaderValue.Parse("application/json");
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonOptions),
            Encoding.UTF8,
            mediaType);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new AnthropicException((int)response.StatusCode, responseText);

        var rawJson = ExtractClaudeText(responseText);

        var result = JsonSerializer.Deserialize<TicketAnalyzeResponse>(rawJson, JsonOptions)
               ?? throw new InvalidOperationException("Failed to deserialize Claude response.");

        if (!TicketAnalyzeResponseValidator.IsValid(result, out var validationError))
        {
            throw new InvalidOperationException($"Claude returned invalid ticket analysis: {validationError}");
        }

        return result;
    }

    private static object CreateRequestBody(string ticketMessage, ClaudeOptions options)
    {
        return new
        {
            model = options.Model,
            max_tokens = options.MaxTokens,
            temperature = options.Temperature,
            system = """
            You are an expert customer support ticket triage assistant.

            Your job is to analyze support tickets and return a strict JSON object.

            Rules:
            - Return JSON only.
            - Do not wrap the JSON in markdown.
            - Do not include explanations outside the JSON.
            - Use only the allowed category and priority values.
            - If the ticket is unclear, choose "General" and "Low" or "Medium".
            - The suggestedReply must be polite, concise, and professional.
            - Never invent facts that are not present in the ticket.
            - If company-specific details are unknown, do not invent them and do not claim the customer can find them on a specific website. Ask a clarifying question instead.

            Allowed categories:
            - Billing: payments, refunds, invoices, subscriptions, duplicate charges.
            - Technical: bugs, errors, crashes, performance, login failures caused by system issues.
            - Account: password reset, profile, account access, permissions, user settings.
            - General: unclear requests, feedback, questions, or anything that does not fit above.

            Priority rules:
            - High: urgent financial issue, security/access issue, system unavailable, angry customer, deadline-sensitive issue.
            - Medium: important issue but not blocking, delayed response acceptable.
            - Low: general question, feedback, small request, no urgency.

            Output JSON shape:
            {
              "category": "Billing | Technical | Account | General",
              "priority": "Low | Medium | High",
              "summary": "One short sentence describing the issue.",
              "suggestedReply": "A short professional customer support reply."
            }

            Example 1:
            Ticket: "I was charged twice this month and I need a refund today."
            Output:
            {
              "category": "Billing",
              "priority": "High",
              "summary": "Customer was charged twice and is requesting an urgent refund.",
              "suggestedReply": "Hi, sorry about the duplicate charge. We will review your billing details and help process the refund as soon as possible."
            }

            Example 2:
            Ticket: "The app crashes every time I try to upload a PDF."
            Output:
            {
              "category": "Technical",
              "priority": "Medium",
              "summary": "Customer cannot upload a PDF because the app crashes.",
              "suggestedReply": "Hi, sorry for the trouble. Please share the PDF size and any error message you see so we can investigate the upload issue."
            }
            """,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"""
                    Analyze this ticket and return only valid JSON.

                    Ticket:
                    {ticketMessage}
                    """
                }
            }
        };
    }

    private static string ExtractClaudeText(string responseText)
    {
        using var document = JsonDocument.Parse(responseText);
        var content = document.RootElement.GetProperty("content");

        if (content.GetArrayLength() == 0)
            throw new InvalidOperationException("Claude returned no content.");

        var text = content[0].GetProperty("text").GetString();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Claude returned empty text.");

        return CleanJson(text);
    }

    private static string CleanJson(string text)
    {
        var trimmed = text.Trim();

        if (trimmed.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            trimmed = trimmed[7..].Trim();

        if (trimmed.StartsWith("```code", StringComparison.OrdinalIgnoreCase))
            trimmed = trimmed[7..].Trim();

        if (trimmed.StartsWith("```"))
            trimmed = trimmed[3..].Trim();

        if (trimmed.EndsWith("```"))
            trimmed = trimmed[..^3].Trim();

        return trimmed;
    }
}
