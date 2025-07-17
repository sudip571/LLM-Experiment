using GeminiDotnet;
using GeminiDotnet.Extensions.AI;
using LLMExperiment;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using System;



var builder = Host.CreateDefaultBuilder(args);

// get your key from Google AI Studio https://makersuite.google.com/app/apikey
string geminiAPIKey = @"AIzaSyBsDWRh33QVIkhPNaOpTYrzowqiDxSWSh#0";
builder.ConfigureServices(services =>
{
    services.AddSingleton<IChatClient>(sp =>
        new GeminiChatClient(new GeminiClientOptions
        {
            ApiKey = geminiAPIKey ?? throw new Exception("Missing API key"),
            ModelId = GeminiModels.Gemini2Flash                                  
        }));

    services.AddHostedService<ChatConsoleApp>();
});

await builder.RunConsoleAsync();
