namespace AdventChatInfrastructure.OpenAIServices
{
    public interface IOpenAIService
    {
        public Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string endpoint, object content);
    }
}
