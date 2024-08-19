using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using System.Text;

namespace AdventChatInfrastructure.RelevantInfoRetrieverServices
{
    public class RelevantInfoRetrieverService: IRelevantInfoRetrieverService
    {
        private readonly IEmbeddingService? _embeddingService;
        private readonly IEmbeddingsStoreService? _embeddingsStoreService;
        //private readonly IOpenAIService? _openAIService;

        public RelevantInfoRetrieverService(IEmbeddingService? embeddingService, IEmbeddingsStoreService? embeddingsStoreService)
        {
            _embeddingService = embeddingService;
            _embeddingsStoreService = embeddingsStoreService;
        }

        public async Task<string> GenerateResponseAsync(string query)
        {
            if (_embeddingService == null || _embeddingsStoreService == null)
            {
                throw new InvalidOperationException("Uno o más servicios requeridos no están inicializados.");
            }

            // Generar embedding para la consulta
            var queryEmbedding = await _embeddingService.GenerateEmbeddingWithCohereForQueryAsync(query);

            // Buscar información relevante en Pinecone
            var queryResults = await _embeddingsStoreService.QueryEmbeddingsAsync(queryEmbedding, topK: 5);

            // Preparar el contexto para OpenAI
            var context = PrepareContext(queryResults);

            // Generar prompt para OpenAI
            var prompt = GeneratePrompt(query, context);

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

        private string GeneratePrompt(string query, string context)
        {
            return $"Contexto:\n{context}\n\nPregunta: {query}\n\nRespuesta:";
        }
    }
}
