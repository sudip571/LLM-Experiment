using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;
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
        private readonly List<ChatMessage> _chatHistory = new();

        public ChatConsoleApp(IChatClient chatClient) => _chatClient = chatClient;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Chat started (empty to exit).");

            while (true)
            {
                Console.Write("\nYou: ");
                var userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput)) break;

                // Add user message to history
                _chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

                var sb = new StringBuilder();
                Console.Write("Gemini: ");
                await foreach (var update in _chatClient.GetStreamingResponseAsync(_chatHistory, cancellationToken:cancellationToken))
                {
                    Console.Write(update.Text);
                    sb.Append(update.Text);
                }

                // Capture the assistant's full reply back into history
                _chatHistory.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }



    //public class ChatConsoleApp : IHostedService
    //{
    //    private readonly IChatClient _chatClient;
    //    public ChatConsoleApp(IChatClient chatClient) => _chatClient = chatClient;

    //    public async Task StartAsync(CancellationToken token)
    //    {
    //        Console.WriteLine("Chat started (empty to exit).");

    //        while (true)
    //        {
    //            Console.Write("\nYou: ");
    //            var input = Console.ReadLine();
    //            if (string.IsNullOrWhiteSpace(input)) break;

    //            Console.Write("Gemini: ");
    //            await foreach (var update in _chatClient.GetStreamingResponseAsync(
    //                       new[] { new ChatMessage(ChatRole.User, input) }, cancellationToken: token))
    //            {
    //                Console.Write(update.Text);
    //            }
    //            Console.WriteLine();
    //        }
    //    }

    //    public Task StopAsync(CancellationToken token) => Task.CompletedTask;
    //}
}

