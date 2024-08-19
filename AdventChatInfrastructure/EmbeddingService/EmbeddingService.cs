using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatDomain;
using AdventChatInfrastructure.OpenAIServices;
using System.Text.Json;

namespace AdventChatInfrastructure.EmbeddingServices
{
    public class EmbeddingService: IEmbeddingService
    {
        private readonly IChunkingService? _chunkingService;
        private readonly IHuggingFaceService? _huggingFaceService;
        private readonly ICohereService? _cohereService;
        private readonly IOpenAIService? _openAIService;
        private HttpMethod? _method= HttpMethod.Post;

        public EmbeddingService(IChunkingService? chunkingService, IOpenAIService? openAIService, IHuggingFaceService huggingFaceService, ICohereService cohereService)
        {
            _chunkingService = chunkingService ?? throw new ArgumentNullException(nameof(chunkingService)); 
            _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
            _huggingFaceService= huggingFaceService ?? throw new ArgumentNullException(nameof(huggingFaceService));
            _cohereService = cohereService ?? throw new ArgumentNullException();

        }


        // This Method guide process embeddings Generation.
        public async Task<List<DocumentRagWithEmbedding>> GenerateEmbeddingsAsync()
        {
            var documentsChunks = await _chunkingService!.CreateDocumentsChunksAsync();            
            var documentsWithEmbeddings = new List<DocumentRagWithEmbedding>();

            var tasks = documentsChunks.Select(async document =>
            {
                var embedding = await CreateEmbeddingFromCohereAsync(document.Content!);
                return new DocumentRagWithEmbedding
                {
                    Id=embedding.Id!,
                    Document = document,
                    Embedding = embedding.Embedding
                };
            });
            var results= await Task.WhenAll(tasks);
            return results.ToList();
           
        }

       
        // This Method access to OpenAIservice to generate embeddings from chunked text
        public async Task<float[]> CreateEmbeddingFromOpenAIAsync(string textQuery)
        {
            if (string.IsNullOrWhiteSpace(textQuery))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(textQuery));
            }

            try
            {
                Task<HttpResponseMessage> embeddingsHttp = _openAIService!.SendRequestAsync(_method!, "/v1/embeddings", textQuery);

                using (HttpResponseMessage response = await embeddingsHttp)
                {
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return ExtractEmbeddingFromOpenAIResponse(jsonResponse);
                }
            }
            catch(HttpRequestException ex)
            {
                throw new Exception("Failed to get embedding from OpenAI", ex);
            }
            
        }

        // This Method access to HuggingFaceservice to generate embeddings from chunked text
        public async Task<float[]> CreateEmbeddingFromHuggingFaceAsync(string textQuery)
        {
            if (string.IsNullOrWhiteSpace(textQuery))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(textQuery));
            }

            try
            {
                Task<HttpResponseMessage> embeddingsHttp = _huggingFaceService!.SendRequestEnbeddingToHFAsync(textQuery);

                using (HttpResponseMessage response = await embeddingsHttp)
                {
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync(); // jsonResponse=[0.09003029763698578] > "Quienes son los Adventistas del Septimo Día?"
                    return ExtractEmbeddingFromHuggingFaceResponse(jsonResponse);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to get embedding from OpenAI", ex);
            }

        }

        // This Method access to Cohereservice to generate embeddings from chunked text
        public async Task<EmbeddingDoc> CreateEmbeddingFromCohereAsync(string textQuery)
        {
            if (string.IsNullOrWhiteSpace(textQuery))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(textQuery));
            }

            try
            {
                Task<HttpResponseMessage> embeddingsHttp = _cohereService!.SendRequestEnbeddingToCohereAsync(textQuery);

                using (HttpResponseMessage response = await embeddingsHttp)
                {
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync(); // jsonResponse=[0.09003029763698578] > "Quienes son los Adventistas del Septimo Día?"
                    return ExtractEmbeddingFromCohereResponse(jsonResponse);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to get embedding from OpenAI", ex);
            }
        }


        // Extract vector from http message from OpenIA API
        private float[] ExtractEmbeddingFromOpenAIResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(jsonResponse));
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement data = root.GetProperty("data")[0];
                    JsonElement embedding = data.GetProperty("embedding");
                    var jsonReturn= JsonSerializer.Deserialize<float[]>(embedding.GetRawText());
                    return jsonReturn!;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to extract embedding from OpenAI response", ex);
            }
        }

        // Extract vector from http message from HuggingFace API
        private float[] ExtractEmbeddingFromHuggingFaceResponse(string jsonResponse) // jsonResponse=[0.09003029763698578]
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(jsonResponse));
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse)) // jsonResponse=[0.09003029763698578]
                {

                    JsonElement embedding = doc.RootElement;                   
                    var stringJson=embedding.GetRawText();
                    var jsonReturn = JsonSerializer.Deserialize<float[]>(stringJson); // salida: jsonReturn=[0.0900303] , entrada: embedding=[0.09003029763698578]
                    return jsonReturn!;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to extract embedding from OpenAI response", ex);
            }
        }

        // Extract vector from http message from Cohere API
        private EmbeddingDoc ExtractEmbeddingFromCohereResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(jsonResponse));
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    JsonElement id=root.GetProperty("id");
                    JsonElement texts = root.GetProperty("texts")[0];
                    JsonElement embeddings = root.GetProperty("embeddings")[0];
                    var jsonIdReturn = JsonSerializer.Deserialize<string>(id.GetRawText());
                    var jsonEmbeddingReturn = JsonSerializer.Deserialize<float[]>(embeddings.GetRawText());
                    var responseExtraction=new EmbeddingDoc() { Id=jsonIdReturn, Embedding=jsonEmbeddingReturn};
                    return responseExtraction!;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to extract embedding from OpenAI response", ex);
            }
        }



        public async Task<float[]> GenerateEmbeddingWithOpenAIForQueryAsync(string query)
        {
            return await CreateEmbeddingFromOpenAIAsync(query);
        }

        public async Task<float[]> GenerateEmbeddingWithHuggingFaceForQueryAsync(string query)
        {
            return await CreateEmbeddingFromHuggingFaceAsync(query);
        }

        public async Task<EmbeddingDoc> GenerateEmbeddingWithCohereForQueryAsync(string query)
        {
            return await CreateEmbeddingFromCohereAsync(query);
        }

    }
}
