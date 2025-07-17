

//Program.cs

/*using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = Kernel.CreateBuilder();
builder.AddOllamaChatCompletion("llama3", new Uri("http://localhost:11434"));

var kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant.");

while (true)
{
    Console.Write("You: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage))
    {
        break;
    }

    history.AddUserMessage(userMessage);

    Console.WriteLine($"Bot: .......");

    var response = await chatService.GetChatMessageContentAsync(history);


    Console.WriteLine($"Bot: {response.Content}");

    history.AddMessage(response.Role, response.Content ?? string.Empty);
}
*/


/*
 * workable code
 
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
    // Register OllamaChatClient directly as IChatClient
    services.AddSingleton<IChatClient>(sp =>
        new OllamaChatClient(new Uri("http://localhost:11434"), "llama3"));

    services.AddHostedService<ChatConsoleApp>();
});

await builder.RunConsoleAsync();

 
 */

/*
 using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LLMExperiment
{
    public class ChatConsoleApp : IHostedService
    {
        private readonly IChatClient _chatClient;

        public ChatConsoleApp(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine("Chat session started. Type your message (empty to exit).");

            var history = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant.")
        };

            while (true)
            {
                Console.Write("You: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                history.Add(new ChatMessage(ChatRole.User, input));

                // Get a streaming response from Ollama
                var responseText = "";

                //var response = await _chatClient.GetResponseAsync(history);
                //Console.WriteLine($"Bot: {response.Text}\n");

                await foreach (var update in _chatClient.GetStreamingResponseAsync(history))
                {
                    Console.Write(update.Text);
                    responseText += update.Text;
                }
                Console.WriteLine();

                history.Add(new ChatMessage(ChatRole.Assistant, responseText));
            }
        }

        public Task StopAsync(System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;
    }
}


 
 
 */