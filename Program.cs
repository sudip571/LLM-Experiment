using LLMExperiment;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using System;



var builder = Host.CreateDefaultBuilder(args);


builder.ConfigureServices(services =>
{
    services.AddSingleton<PdfService>();
    // Register OllamaChatClient directly as IChatClient
    services.AddSingleton<IChatClient>(sp =>
        new OllamaChatClient(new Uri("http://localhost:11434"), "llama3"));

    services.AddSingleton<IEmbeddingClient>(sp =>
    new OllamaEmbeddingClient(new Uri("http://localhost:11434"), "nomic-embed-text"));

    services.AddHostedService<ChatConsoleApp>();
});

await builder.RunConsoleAsync();
