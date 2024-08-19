using AdventChatInfrastructure.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AdventChatInfrastructure.OpenAIServices
{
    public class OpenAIService: IOpenAIService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly OpenAISettings _settings;

        public OpenAIService(IHttpClientFactory clientFactory, IOptions<OpenAISettings> settings)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
        }


        public async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string endpoint, object content)
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new InvalidOperationException("API Key is missing.");
            }

            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));
            }

            var client = _clientFactory.CreateClient("OpenAI");


            var stringUrl = _settings.BaseUrl + endpoint;

            Uri requestUri;
            if (Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
            {
                requestUri = new Uri(endpoint);
            }
            else
            {
                requestUri = new Uri(new Uri(_settings.BaseUrl!), endpoint);
            }

            

            HttpResponseMessage? response=null;
            int maxRetries = 3;
            int delay = 1000; // 1 segundo
            for (int i = 0; i < maxRetries; i++)
            {
                using var request = new HttpRequestMessage(method, requestUri);

                if (endpoint.Contains("embeddings"))
                {
                    // Construye el objeto JSON final con el prompt y otros parámetros
                    var requestBody = new
                    {
                        model = _settings.ModelEmbeddingName,
                        input = content,
                    };

                    // Serializa el objeto a una cadena JSON
                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    request.Content = requestContent;
                }
                else
                {
                    // Construye el objeto JSON final con el prompt y otros parámetros
                    var requestBody = new
                    {
                        model = _settings.model,
                        messages = new[]{ 
                             new{ 
                                role = "user", 
                                content = new[] { 
                                    new {
                                        type="text", 
                                        text= content 
                                    } 
                                } 
                             } 
                        },
                        max_tokens = _settings.max_tokens,
                        temperature = _settings.temperature,
                        top_p = _settings.top_p,
                        frequency_penalty = _settings.frequency_penalty,
                        presence_penalty = _settings.presence_penalty,
                        response_format = new { type ="text"}
                    };

                    // Serializa el objeto a una cadena JSON
                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    request.Content = requestContent;
                }

                request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");

                try
                {
                    response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return response; // Si tiene éxito, salir del bucle y retornar la respuesta
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    if (i == maxRetries - 1)
                    {
                        // Si es el último intento, lanzar la excepción
                        throw new Exception("\"Error 429: Too Many Requests. Implementar lógica de reintento.\"", ex);
                    }
                    Console.WriteLine($"Error 429: Too Many Requests. Reintentando en {delay / 1000} segundos...");
                    await Task.Delay(delay);
                    delay *= 2; // Incremento exponencial del retraso
                    
                }
                catch (HttpRequestException ex) when (response != null && response.StatusCode == (System.Net.HttpStatusCode)503)
                {
                    // Manejar el error 503 (Service Unavailable) aquí
                    throw new Exception("Error 503: Service Unavailable. Implementar lógica de reintento.");
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
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                    throw; // Re-lanza la excepción para manejarla en el lugar que llame a este método
                }

            }
            // Manejar el error 429 (Too Many Requests) aquí
            throw new Exception("Error 429: Too Many Requests. Implementar lógica de reintento.");
        }
    }
}
