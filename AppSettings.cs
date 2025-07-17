using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMExperiment
{
    public static class AppSettings
    {
        public static string ChatModel = "llama3"; // Ollama chat model
        //public static string ChatModel = "mistral"; // Ollama chat model
        public static string EmbeddingModel = "nomic-embed-text"; // Embedding model
        public const string CollectionName = "pdf-memory";
    }

}
