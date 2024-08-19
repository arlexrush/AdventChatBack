namespace AdventChatInfrastructure.Models
{
    public class OpenAISettings
    {
        public string? ApiKey { get; set; }             // Apikey de la API
        public string? ModelEmbeddingName { get; set; } // El modelo de Embedding
        public string? BaseUrl { get; set; }            // Url base de la API
        public string? model { get; set; }              // El modelo de lenguaje a utilizar.
        public string? max_tokens { get; set; }         // Número máximo de tokens en la respuesta generada.
        public string? temperature { get; set; }        // Ajusta la creatividad de la respuesta. Entre 0 y 1.
        public string? top_p { get; set; }              // Controla la diversidad de la respuesta.
        public string? frequency_penalty { get; set; }  // Penaliza la repetición de frases comunes.
        public string? presence_penalty {  get; set; }  // Penaliza la repetición de temas ya presentes en el prompt.   
    }
}
