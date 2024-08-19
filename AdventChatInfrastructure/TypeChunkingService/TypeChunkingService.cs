using AdventChatApplication.Contracts.Infrastructure;
using AdventChatInfrastructure.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace AdventChatInfrastructure.TypeChunkingServices
{
    public class TypeChunkingService: ITypeChunkingService
    {
        private readonly ChunkingSettings? _chunkingsettings;

        public TypeChunkingService(IOptions<ChunkingSettings>? chunkingsettings)
        {
            _chunkingsettings = chunkingsettings!.Value;
        }

        public List<string> SemanticChunkText(string text)
        {
            var chunks = new List<string>();

            // divide text in paragraphs
            var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new List<string>();
            int currentSize = 0;

            foreach (var paragraph in paragraphs)
            {
                // Verifico que el parrafo se encuentre entre los limites definidos en chunkingSettings
                if (currentSize + paragraph.Length > _chunkingsettings!.MaxChunkSize && currentSize > _chunkingsettings.MinChunkSize)
                {
                    chunks.Add(string.Join(" ", currentChunk));
                    currentChunk.Clear();
                    currentSize = 0;
                }

                if (paragraph.Length > _chunkingsettings.MaxChunkSize)
                {
                    // Si el párrafo es muy largo, lo dividimos en oraciones
                    var sentences = Regex.Split(paragraph, @"(?<=[.!?])\s+");
                    foreach (var sentence in sentences)
                    {
                        if (currentSize + sentence.Length > _chunkingsettings.MaxChunkSize && currentSize > _chunkingsettings.MinChunkSize)
                        {
                            chunks.Add(string.Join(" ", currentChunk));
                            currentChunk.Clear();
                            currentSize = 0;
                        }
                        currentChunk.Add(sentence);
                        currentSize += sentence.Length;
                    }
                }
                else
                {
                    currentChunk.Add(paragraph);
                    currentSize += paragraph.Length;
                }
            }

            if (currentChunk.Any())
            {
                chunks.Add(string.Join(" ", currentChunk));
            }

            return chunks;
        }
    }
}
