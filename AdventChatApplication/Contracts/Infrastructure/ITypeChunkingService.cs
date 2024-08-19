namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface ITypeChunkingService
    {
        public List<string> SemanticChunkText(string text);
    }
}
