using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Graphics;

namespace LLMExperiment
{
    public class PdfService
    {
        private readonly string _pdfFolderPath;
        private readonly IEmbeddingClient _embeddingClient;

        private readonly List<(string Chunk, float[] Vector)> _vectorStore = new();

        public PdfService(IEmbeddingClient embeddingClient, string pdfFolderPath = "pdfs")
        {
            _embeddingClient = embeddingClient;
            //_pdfFolderPath = Path.Combine(Directory.GetCurrentDirectory(), pdfFolderPath);
            _pdfFolderPath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "", pdfFolderPath);
        }

        public async Task IndexPdfsAsync()
        {
            if (!Directory.Exists(_pdfFolderPath)) return;

            var pdfFiles = Directory.GetFiles(_pdfFolderPath, "*.pdf");

            foreach (var file in pdfFiles)
            {
                var text = new StringBuilder();
                using var doc = PdfDocument.Open(file);
                foreach (var page in doc.GetPages()) text.AppendLine(page.Text);

                var chunks = ChunkText(text.ToString());
                foreach (var chunk in chunks)
                {
                    var embedding = await _embeddingClient.GetEmbeddingAsync(chunk);
                    _vectorStore.Add((chunk, embedding));
                }
            }

            Console.WriteLine($"Indexed {_vectorStore.Count} chunks from {pdfFiles.Length} PDF(s).");
        }

        public async Task<List<string>> SearchRelevantChunksAsync(string query, int topK = 5)
        {
            var queryVec = await _embeddingClient.GetEmbeddingAsync(query);

            var results = _vectorStore
                .Select(chunk => new
                {
                    Text = chunk.Chunk,
                    Score = CosineSimilarity(queryVec, chunk.Vector)
                })
                .OrderByDescending(r => r.Score)
                .Take(topK)
                .Select(r => r.Text)
                .ToList();

            return results;
        }

        private static double CosineSimilarity(float[] vecA, float[] vecB)
        {
            double dot = 0.0, magA = 0.0, magB = 0.0;
            for (int i = 0; i < vecA.Length; i++)
            {
                dot += vecA[i] * vecB[i];
                magA += vecA[i] * vecA[i];
                magB += vecB[i] * vecB[i];
            }
            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-8);
        }

        private static List<string> ChunkText(string text, int maxWords = 200)
        {
            var words = text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();
            for (int i = 0; i < words.Length; i += maxWords)
            {
                chunks.Add(string.Join(' ', words.Skip(i).Take(maxWords)));
            }
            return chunks;
        }
    }
}
