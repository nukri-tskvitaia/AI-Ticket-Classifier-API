using AiTicketClassifier.Api.Models;

namespace AiTicketClassifier.Api.Validation;

public static class TicketAnalyzeResponseValidator
{
    private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "Billing",
        "Technical",
        "Account",
        "General"
    };

    private static readonly HashSet<string> AllowedPriorities = new(StringComparer.OrdinalIgnoreCase)
    {
        "Low",
        "Medium",
        "High"
    };

    public static bool IsValid(TicketAnalyzeResponse response, out string error)
    {
        if (string.IsNullOrWhiteSpace(response.Category))
        {
            error = "Category is required.";
            return false;
        }

        if (!AllowedCategories.Contains(response.Category))
        {
            error = $"Invalid category: {response.Category}.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Priority))
        {
            error = "Priority is required.";
            return false;
        }

        if (!AllowedPriorities.Contains(response.Priority))
        {
            error = $"Invalid priority: {response.Priority}.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Summary))
        {
            error = "Summary is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.SuggestedReply))
        {
            error = "SuggestedReply is required.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
