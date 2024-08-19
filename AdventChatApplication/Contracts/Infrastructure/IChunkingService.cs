using AdventChatDomain;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IChunkingService
    {
        public Task<List<DocumentRag>> CreateDocumentsChunksAsync();
    }
}
