using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using System.Text;

namespace AdventChatInfrastructure.RelevantInfoRetrieverServices
{
    public class RelevantInfoRetrieverService: IRelevantInfoRetrieverService
    {
        private readonly IEmbeddingService? _embeddingService;
        private readonly IEmbeddingsStoreService? _embeddingsStoreService;
        private readonly ITypeChunkingService? _typeChunkingService;
        //private readonly IOpenAIService? _openAIService;

        public RelevantInfoRetrieverService(IEmbeddingService? embeddingService, IEmbeddingsStoreService? embeddingsStoreService, ITypeChunkingService typeChunkingService)
        {
            _embeddingService = embeddingService;
            _embeddingsStoreService = embeddingsStoreService;
            _typeChunkingService = typeChunkingService;
        }

        public async Task<string> GenerateResponseAsync(string userPrompt)
        {


            if (_embeddingService == null || _embeddingsStoreService == null)
            {
                throw new InvalidOperationException("Uno o más servicios requeridos no están inicializados.");
            }

            var promptsFromContext=new List<string>();

            // Chunking userPrompt
            var chunksFromPrompt= _typeChunkingService!.SemanticChunkText(userPrompt);

            foreach (string chunk in chunksFromPrompt)
            {
                // Generar embedding para la consulta
                var embeddingFromQuery = await _embeddingService.GenerateEmbeddingWithCohereForQueryAsync(chunk);

                // Buscar información relevante en Pinecone
                var queryResults = await _embeddingsStoreService.QueryEmbeddingsAsync(embeddingFromQuery, topK: 10);

                // Preparar el contexto para OpenAI
                var contextN = PrepareContext(queryResults);

                promptsFromContext.Add(contextN);
            }
            
            // Generar prompt para OpenAI
            var prompt = GeneratePrompt(userPrompt, promptsFromContext);

            // Obtener respuesta de OpenAI
            //var response = await _openAIService.GetCompletionAsync(prompt);

            var response= prompt;

            return response;
        }

        private string PrepareContext(List<PineconeQueryResult> queryResults)
        {
            var contextBuilder = new StringBuilder();
            foreach (var result in queryResults)
            {
                if (result.Metadata.TryGetValue("text", out var text))
                {
                    contextBuilder.AppendLine(text.ToString());
                }
            }
            return contextBuilder.ToString();
        }

        private string GeneratePrompt(string userPrompt, List<string> promptsFromContext)
        {
            string context="";
            foreach (string promptString in promptsFromContext)
            {
                context = context + $"\n{promptString}\n";
            }

            return $"Contexto:\n{context}\n\nPregunta: {userPrompt}\n\nRespuesta:";
        }
    }
}
