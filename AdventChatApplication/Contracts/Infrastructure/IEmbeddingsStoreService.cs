using AdventChatApplication.Models;
using AdventChatDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IEmbeddingsStoreService
    {
        Task<bool> StoreEmbeddingAsync(List<DocumentRagWithEmbedding> documentsRagWithEmbedding);
        Task<List<PineconeQueryResult>> QueryEmbeddingsAsync(EmbeddingDoc queryEmbedding, int topK);
        Task<bool> UpdateEmbeddingAsync(List<DocumentRagWithEmbedding> vectors);
        Task<bool> DeleteEmbeddingAsync(List<string> ids);
        Task<Dictionary<string, object>> GetEmbeddingMetadataAsync(string id);
    }
}
