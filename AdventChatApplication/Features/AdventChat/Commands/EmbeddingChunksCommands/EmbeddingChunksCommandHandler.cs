using AdventChatApplication.Contracts.Infrastructure;
using MediatR;

namespace AdventChatApplication.Features.AdventChat.Commands.EmbeddingChunksCommands
{
    public class EmbeddingChunksCommandHandler : IRequestHandler<EmbeddingChunksCommand, Unit>
    {
        private readonly IChunkingService? _chunkingService;
        private readonly IEmbeddingService? _embeddingService;
        private readonly IEmbeddingsStoreService? _embeddingsStoreService;

        public EmbeddingChunksCommandHandler(IChunkingService? chunkingService, IEmbeddingService? embeddingService, IEmbeddingsStoreService? embeddingsStoreService)
        {
            _chunkingService = chunkingService;
            _embeddingService = embeddingService;
            _embeddingsStoreService = embeddingsStoreService;
        }

        public async Task<Unit> Handle(EmbeddingChunksCommand request, CancellationToken cancellationToken)
        {
            var embeddingsFromChunks = await _embeddingService!.GenerateEmbeddingsAsync();
            var EmbedingsStored=await _embeddingsStoreService!.StoreEmbeddingAsync(embeddingsFromChunks);

            return Unit.Value;
        }
    }
}
