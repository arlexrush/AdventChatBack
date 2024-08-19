using AdventChatApplication.Models;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IProcessingDataService
    {
        public Task<List<FileRag>> ProcessingFilesWithGoogleFirebase();
    }
}
