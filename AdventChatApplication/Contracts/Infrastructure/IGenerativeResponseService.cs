namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IGenerativeResponseService
    {
        // This method is for try to get Context from Vectorial Database using user prompt
        public Task<string> GetPromptFromContext(string prompt);

        // This method is for to get response from OpenAI using contextual prompt improved with vectorial embedding database
        public Task<string> GetResponseFromOpenAI(string prompt);
    }
}
