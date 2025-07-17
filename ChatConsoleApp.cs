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
        private readonly PdfService _pdfService;

        public ChatConsoleApp(IChatClient chatClient, PdfService pdfService)
        {
            _chatClient = chatClient;
            _pdfService = pdfService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Choose mode:");
            Console.WriteLine("1. Chat with AI");
            Console.WriteLine("2. Ask questions based on your PDF documents");
            Console.Write("Enter choice (1 or 2): ");
            var mode = Console.ReadLine()?.Trim();

            if (mode == "1")
            {
                await StartChatAsync();
            }
            else if (mode == "2")
            {
                await StartDocumentQnAAsync();
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        private async Task StartChatAsync()
        {
            Console.WriteLine("\nChat mode. Type your message (empty to exit).");

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

                var responseText = "";

                await foreach (var update in _chatClient.GetStreamingResponseAsync(history))
                {
                    Console.Write(update.Text);
                    responseText += update.Text;
                }
                Console.WriteLine();

                history.Add(new ChatMessage(ChatRole.Assistant, responseText));
            }
        }

        private async Task StartDocumentQnAAsync()
        {
            await _pdfService.IndexPdfsAsync(); // Index once at startup

            while (true)
            {
                Console.Write("Ask a question (empty to exit): ");
                var question = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(question)) break;

                var topChunks = await _pdfService.SearchRelevantChunksAsync(question);

                var context = string.Join("\n---\n", topChunks);

                var history = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "Answer the question using only the provided document context."),
            new ChatMessage(ChatRole.User, $"Context:\n{context}\n\nQuestion:\n{question}")
        };

                var responseText = "";

                await foreach (var update in _chatClient.GetStreamingResponseAsync(history))
                {
                    Console.Write(update.Text);
                    responseText += update.Text;
                }
                Console.WriteLine();
            }
        }
        public Task StopAsync(System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

