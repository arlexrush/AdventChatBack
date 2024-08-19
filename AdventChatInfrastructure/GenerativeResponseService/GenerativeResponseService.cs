using AdventChatApplication.Contracts.Infrastructure;
using AdventChatInfrastructure.OpenAIServices;
using System.Text.Json;

namespace AdventChatInfrastructure.GenerativeResponseServices
{
    public class GenerativeResponseService: IGenerativeResponseService
    {
        private readonly IOpenAIService? _openAIService;
        private readonly IRelevantInfoRetrieverService? _relevantInfoRetrieverService;

        public GenerativeResponseService(IOpenAIService? openAIService, IRelevantInfoRetrieverService? relevantInfoRetrieverService)
        {
            _openAIService = openAIService;
            _relevantInfoRetrieverService = relevantInfoRetrieverService;
        }

        public async Task<string>GetPromptFromContext(string prompt)
        {
            var response= await _relevantInfoRetrieverService!.GenerateResponseAsync(prompt);
            return response;
        }

        public async Task<string>GetResponseFromOpenAI(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(prompt));
            }

            try
            {            
                var responseHttp=await _openAIService!.SendRequestAsync(HttpMethod.Post, "/v1/chat/completions", prompt);
                using (HttpResponseMessage response = responseHttp)
                {
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return ExtractContentFromResponse(jsonResponse);
                }
            }catch (HttpRequestException ex)
            {
                throw new Exception("Failed to get Response from OpenAI", ex);
            }
            
        }

        private string ExtractContentFromResponse(string jsonResponse)
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
                    JsonElement content = data.GetProperty("embedding");
                    var jsonReturn = JsonSerializer.Deserialize<string>(content.GetRawText());
                    return jsonReturn!;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to extract embedding from OpenAI response", ex);
            }
        }
    }
}
