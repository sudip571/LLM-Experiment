using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LLMExperiment
{
    public interface IEmbeddingClient
    {
        Task<float[]> GetEmbeddingAsync(string text);
    }

    public class OllamaEmbeddingClient : IEmbeddingClient
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OllamaEmbeddingClient(Uri baseAddress, string model)
        {
            _http = new HttpClient { BaseAddress = baseAddress };
            _model = model;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var payload = new { model = _model, prompt = text };
            var response = await _http.PostAsJsonAsync("/api/embeddings", payload);
            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return result?.Embedding ?? Array.Empty<float>();
        }

        private class EmbeddingResponse
        {
            public float[] Embedding { get; set; }
        }
    }
}
