using AiTicketClassifier.Api.Options;
using AiTicketClassifier.Api.Services;
using AiTicketClassifier.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<ClaudeOptions>()
    .Bind(builder.Configuration.GetSection(ClaudeOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), "Claude API key is required.")
    .ValidateOnStart();

builder.Services.AddHttpClient<ClaudeTicketAnalyzer>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();

/*
  "Claude": {
    "ApiKey": "your_cloude_api_key",
    "Version": "model_year_value",
    "Model": "model_id_value",
    "MaxTokens": max_tokens_value,
    "Temperature": temperature_value,
    "MessagesEndpointUrl": "messages_endpoint_url_value"
  }
*/