using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatDomain;
using AdventChatInfrastructure.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AdventChatInfrastructure.EmbeddingStorageServices
{
    public class PineconeService : IPineconeService
    {
        private readonly IHttpClientFactory? _clientFactory;
        private readonly PineconeSettings? _settings;

        public PineconeService(IHttpClientFactory? clientFactory, IOptions<PineconeSettings>? settings)
        {
            _clientFactory = clientFactory;
            _settings = settings!.Value;
        }

        private HttpClient CreateClient()
        {
            var client = _clientFactory!.CreateClient();
            client.DefaultRequestHeaders.Add("Api-Key", _settings!.ApiKey);
            return client;
        }

        // Inserción y Actualización de Vectores
        public async Task<bool> UpsertVectorsAsync(List<DocumentRagWithEmbedding> vectors)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings!.BaseUrl}/vectors/upsert");

                      
            var payload = new {vectors = vectors.Select(v => new {id = v.Id, values = v.Embedding, metadata = v.Document!.Metadata}).ToList()};

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


        //buscar los vectores más similares a un vector de consulta dado en tu índice de Pinecone. topK= Numero de vectores que se quieren en la respuesta
        public async Task<List<PineconeQueryResult>> QueryAsync(float[] queryVector, int topK=10)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_settings!.BaseUrl}/query"));

            var payload = new
            {
                vector = queryVector,
                topK = topK,
                includeValues=true,
                includeMetadata = true


            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<PineconeQueryResult>();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<QueryResponse>(content);

            if (result?.Matches != null&&result!.Matches!.Any())
            {
                var responsesToClient = result!.Matches!.Select(m => new PineconeQueryResult(m.Id, m.Score, m.Values!, m.Metadata!)).ToList();

                return responsesToClient;
            }
            return new List<PineconeQueryResult>();
        }

        // Borrar vectores por su id
        public async Task<bool> DeleteVectorsAsync(List<string> ids)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings!.BaseUrl}/vectors/delete");

            var payload = new { ids = ids };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Sirve para recuperar los metadatos de un vector dado con un id como parametro.
        public async Task<Dictionary<string, object>> FetchVectorAsync(string id)
        {
            var client = CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings!.BaseUrl}/vectors/fetch?ids={id}");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null!;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FetchResponse>(content);

            return result!.Vectors.FirstOrDefault().Value?.Metadata!;
        }







    }
}
