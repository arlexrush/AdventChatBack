using AdventChatApplication.Contracts.Infrastructure;
using AdventChatInfrastructure.HuggingFaceServices;
using AdventChatInfrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdventChatInfrastructure.CohereServices
{
    public class CohereService:ICohereService
    {
        private readonly IHttpClientFactory? _clientFactory;
        private readonly CohereSettings? _settings;
        private readonly ILogger<CohereService>? _logger;

        public CohereService(IHttpClientFactory? clientFactory, IOptions<CohereSettings>? settings, ILogger<CohereService>? logger)
        {
            _clientFactory = clientFactory;
            _settings = settings!.Value;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> SendRequestEnbeddingToCohereAsync(string content, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_settings!.ApiKey))
            {
                throw new InvalidOperationException("API Key is missing.");
            }
            var client = _clientFactory!.CreateClient("Cohere");

            HttpResponseMessage? response = null;
            int maxRetries = 3;
            int delay = 1000; // 1 segundo
            for (int i = 0; i < maxRetries; i++)
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_settings.ApiUrl!));
                // Construye el objeto JSON final con el prompt y otros parámetros
                var requestBody = new
                {
                    model= "embed-multilingual-v3.0",
                    texts= new[] { content },
                    input_type= "search_document",
                    embedding_types= new float[] { },
                };

                // Serializa el objeto a una cadena JSON
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Agrega el contenido del body del json al request
                request.Content = requestContent;

                // Agrega las credenciales de acceso a la API de Hugging Face
                request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
                try
                {
                    response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return response; // Si tiene éxito, salir del bucle y retornar la respuesta
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    _logger!.LogWarning("Error 429: Too Many Requests. Implementing retry logic.");
                    if (i == maxRetries - 1) throw;
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)503)
                {
                    _logger!.LogWarning($"Error 503: Service Unavailable. Retrying in {delay / 1000} seconds...");
                    if (i == maxRetries - 1) throw;
                    await Task.Delay(delay, cancellationToken);
                    delay *= 2; // Exponential backoff
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)403)
                {
                    // Manejar el error 403 (Forbidden) aquí
                    throw new Exception("Error 403: Forbidden. Verificar claves de API y permisos.");
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)400)
                {
                    // Manejar el error 400 (Bad Request) aquí
                    throw new Exception("Error 400: Bad Request. Revisar formato y contenido del cuerpo de la solicitud.");
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)401)
                {
                    // Manejar el error 401 (Unauthorized) aquí
                    throw new Exception("Error 401: Unauthorized. Verificar clave de API.");
                }
                catch (HttpRequestException ex)
                {
                    var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response received";
                    throw new HttpRequestException($"Error: {ex.Message}. Response: {errorContent}", ex);
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, "Unexpected error occurred while comparing sentences.");
                    throw;
                }

            }

            // Manejar el error 503 (Service Unavailable) aquí
            throw new Exception("Error 503: Service Unavailable. Implementar lógica de reintento.");
        }

    }
}
