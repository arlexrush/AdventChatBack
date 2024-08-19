namespace AdventChatDomain
{
    public class DocumentRagWithEmbedding
    {
        public required string Id { get; set; }
        public DocumentRag? Document { get; set; }
        public float[]? Embedding { get; set; }
    }
}
