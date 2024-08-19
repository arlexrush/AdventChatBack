using AdventChatApplication.Models;
using AdventChatDomain;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IEmbeddingService
    {
        public Task<List<DocumentRagWithEmbedding>> GenerateEmbeddingsAsync();
        public Task<float[]> GenerateEmbeddingWithOpenAIForQueryAsync(string query);
        public Task<float[]> GenerateEmbeddingWithHuggingFaceForQueryAsync(string query);
        public Task<EmbeddingDoc> GenerateEmbeddingWithCohereForQueryAsync(string query);
    }
}
