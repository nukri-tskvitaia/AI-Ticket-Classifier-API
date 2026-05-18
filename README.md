# AI Ticket Classifier API

A lightweight ASP.NET Core Web API that uses Anthropic Claude to analyze customer support tickets and return structured AI-generated responses.

The API classifies tickets by category and priority, generates a concise summary, and creates a professional suggested customer reply.

---

# Features

- ASP.NET Core Web API
- Claude AI integration
- Ticket classification
- Priority detection
- AI-generated summaries
- AI-generated suggested replies
- Strongly typed configuration using `IOptions`
- Request validation
- AI response validation
- Swagger/OpenAPI support
- Clean architecture structure
- `CancellationToken` support
- Safe JSON parsing and cleanup

---

# Example Request

```json
{
  "message": "I forgot my password and cannot access my account."
}
```

# Example Response

```json
{
  "category": "Account",
  "priority": "High",
  "summary": "Customer cannot access their account due to a forgotten password.",
  "suggestedReply": "Hi, we can help you regain access to your account. Please click the 'Forgot Password' link on the login page to reset your password, or let us know if you need further assistance."
}
```

---

# Technologies Used

- ASP.NET Core 9 Web API
- C#
- Anthropic Claude API
- Swagger / OpenAPI
- System.Text.Json
- Options Pattern (`IOptions<T>`)

---

# Project Structure

```text
Controllers/
Models/
Options/
Services/
Validation/
Exceptions/
```

---

# Configuration

Store configuration securely using User Secrets or environment variables.

Example configuration:

```json
{
  "Claude": {
    "ApiKey": "your_claude_api_key",
    "Version": "2023-06-01",
    "Model": "claude-haiku-4-5-20251001",
    "MaxTokens": 700,
    "Temperature": 0.1,
    "MessagesEndpointUrl": "https://api.anthropic.com/v1/messages"
  }
}
```

---

# Running the Project

## Clone the repository

```bash
git clone <your_repo_url>
```

## Navigate to project

```bash
cd AiTicketClassifier.Api
```

## Restore packages

```bash
dotnet restore
```

## Run the API

```bash
dotnet run
```

Swagger UI will open automatically.

---

# API Endpoint

## Analyze Ticket

```http
POST /api/tickets/analyze
```

---

# Validation

The application validates:

- Request model input
- Allowed categories
- Allowed priorities
- Required AI response fields
- JSON deserialization integrity

This helps ensure the API returns only valid structured responses even though LLM outputs are probabilistic by nature.

---

# Notes

This project demonstrates:

- AI API integration
- Prompt engineering
- Structured AI response handling
- Backend validation techniques
- Production-oriented API design principles

The project intentionally keeps the architecture lightweight while applying clean and scalable engineering practices.