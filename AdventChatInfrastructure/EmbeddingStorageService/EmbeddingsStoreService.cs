using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatDomain;

namespace AdventChatInfrastructure.EmbeddingStorageServices
{
    public class EmbeddingsStoreService: IEmbeddingsStoreService
    {
        private readonly IPineconeService _pineconeService;

        public EmbeddingsStoreService(IPineconeService pineconeService)
        {
            _pineconeService = pineconeService;
        }

        // Acces to Database, save or update vectors
        public async Task<bool> StoreEmbeddingAsync(List<DocumentRagWithEmbedding> documentRagWithEmbedding)
        {
            return await _pineconeService.UpsertVectorsAsync(documentRagWithEmbedding);
        }

        // Acces to Database, Query by vector
        public async Task<List<PineconeQueryResult>> QueryEmbeddingsAsync(EmbeddingDoc queryEmbedding, int topK)
        {
            return await _pineconeService.QueryAsync(queryEmbedding.Embedding!, topK);
        }

        // Acces to Database, update vectors
        public async Task<bool> UpdateEmbeddingAsync(List<DocumentRagWithEmbedding> vectors)
        {
            return await _pineconeService.UpsertVectorsAsync(vectors);
        }

        // Acces to Database, delete vector by ids
        public async Task<bool> DeleteEmbeddingAsync(List<string> ids)
        {
            return await _pineconeService.DeleteVectorsAsync(ids);
        }

        // Acces to Database, get Metadata vector by id
        public async Task<Dictionary<string, object>> GetEmbeddingMetadataAsync(string id)
        {
            return await _pineconeService.FetchVectorAsync(id);
        }

        // Acces to Database, save an vector
        public async Task<bool> StoreEmbeddingAsync(DocumentRagWithEmbedding documentRagWithEmbedding)
        {
            if (!documentRagWithEmbedding.Document!.Content!.Any())
            {
                var vectors = new List<DocumentRagWithEmbedding>();
                vectors.Add(documentRagWithEmbedding);
                return await _pineconeService.UpsertVectorsAsync(vectors);
            }else
            {
                return false;
            }
            
        }
    }
}
