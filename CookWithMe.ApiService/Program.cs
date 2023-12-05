using Azure.AI.OpenAI;
using Azure;
using CookWithMe.ApiService.enums;
using Microsoft.Extensions.Options;
using CookWithMe.ApiService;
using System;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Google.Protobuf.Compiler;
using CookWithMe.ApiService.Model;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("settings"));

builder.Services.AddSingleton(provider =>
{
    Settings settings = provider.GetRequiredService<IOptions<Settings>>().Value;

    OpenAIClient client = settings.Type == OpenAIType.Azure
        ? new OpenAIClient(new Uri(settings.Endpoint!), new AzureKeyCredential(settings.Key))
        : new OpenAIClient(settings.Key!);

    return client;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
});

app.MapPost("/recipe", async (HttpRequest request) =>
{
    var body = await request.ReadFromJsonAsync<Recipe>();

    var key = builder.Configuration.GetSection("settings").GetSection("key");
    string endpoint = "https://gowopenai.openai.azure.com/";
    var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key.ToString()));

    ChatCompletionsOptions completionsOptions = new()
    {
        DeploymentName = "gpt-35-model",

        Temperature = (float)0.7,
        MaxTokens = 800,
        NucleusSamplingFactor = (float)0.95,
        FrequencyPenalty = 0,
        PresencePenalty = 0,
    };
    completionsOptions.Messages.Add(new(ChatRole.User, $"Give recipe suggestion for {body.SelectedType[0]} , {body.SelectedType[1]} ,  using {body.Ingredients} "));

    Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync(completionsOptions);
    return completionsResponse.Value.Choices.First().Message.Content.ToString();

});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
