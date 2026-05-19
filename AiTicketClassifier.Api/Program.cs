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