using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatDomain;
using AdventChatInfrastructure.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AdventChatInfrastructure.ChunkingServices
{
    public class ChunkingService: IChunkingService
    {
        private readonly ChunkingSettings? _chunkingsettings;
        private readonly IProcessingDataService _processingDataService;
        private readonly ITypeChunkingService _typeChunkingService;

        public ChunkingService(IOptions<ChunkingSettings>? chunkingsettings, IProcessingDataService processingDataService, ITypeChunkingService typeChunkingService)
        {
            _chunkingsettings = chunkingsettings!.Value;
            _processingDataService = processingDataService;
            _typeChunkingService = typeChunkingService;
        }

        public async Task<List<DocumentRag>> CreateDocumentsChunksAsync()
        {
            List<FileRag> filesRag = await _processingDataService.ProcessingFilesWithGoogleFirebase();

            var documents = new List<DocumentRag>();

            foreach (var fileRag in filesRag)
            {
                var sourceFileName=fileRag.FileName;
                var chunks = _typeChunkingService.SemanticChunkText(fileRag.FileContent!);
                for (int i = 0; i < chunks.Count; i++)
                {
                    var documentRag = new DocumentRag
                    {
                        Content = chunks[i],
                        Metadata = new Dictionary<string, string>
                        {
                            { "source", sourceFileName! },
                            { "chunk_id", ComputeHash((i + 1).ToString())},
                            { "total_chunks", chunks.Count.ToString() },
                            { "char_count", chunks[i].Length.ToString() },
                            { "word_count", CountWords(chunks[i]).ToString() },
                            { "created_at", DateTime.UtcNow.ToString("o") },
                            { "hash", ComputeHash(chunks[i]) }
                        }
                    };
                    documents.Add(documentRag);
                }
            }
            return documents;
        }

        private int CountWords(string text)
        {
            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }        
    }
}
