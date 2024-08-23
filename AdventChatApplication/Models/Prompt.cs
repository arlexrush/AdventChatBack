namespace AdventChatApplication.Models
{
    public class Prompt
    {
        public string? PromptId { get; set; }
        public string? UserId { get; set;}
        public string? Speaker {  get; set; } // maybe userName or LLMBot
        public string? DialogId { get; set; }
        public DateTime CreatePrompt { get; set; }
        public string? ContentPrompt { get; set; }

    }
}
