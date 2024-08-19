using AdventChatApplication.Models;
using AdventChatDomain;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IPineconeService
    {
        public Task<bool> UpsertVectorsAsync(List<DocumentRagWithEmbedding> vectors);
        public Task<List<PineconeQueryResult>> QueryAsync(float[] queryVector, int topK);
        public Task<bool> DeleteVectorsAsync(List<string> ids);
        public Task<Dictionary<string, object>> FetchVectorAsync(string id);
    }
}
