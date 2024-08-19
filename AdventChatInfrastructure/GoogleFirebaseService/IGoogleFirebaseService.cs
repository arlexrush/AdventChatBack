using AdventChatApplication.Models;

namespace AdventChatInfrastructure.GoogleFirebaseServices
{
    public interface IGoogleFirebaseService
    {
        public Task<List<FileRag>> GetAllFilesContentFromGoogle();

    }
}
