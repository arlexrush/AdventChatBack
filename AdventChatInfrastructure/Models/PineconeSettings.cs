namespace AdventChatInfrastructure.Models
{
    public class PineconeSettings
    {
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? IndexName { get; set; }
        public string? Dimension {  get; set; }
        public string? SimilarityMetric { get; set; }
    }
}
