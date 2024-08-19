using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatInfrastructure.GoogleFirebaseServices;

namespace AdventChatInfrastructure.ProcessingDataServices
{
    public class ProcessingDataService: IProcessingDataService
    {

        private readonly IGoogleFirebaseService _googleFirebaseService;
       

        public ProcessingDataService( IGoogleFirebaseService googleFirebaseService) 
        {
            _googleFirebaseService= googleFirebaseService;
        }

        public async Task<List<FileRag>> ProcessingFilesWithGoogleFirebase()
        {
            return await _googleFirebaseService.GetAllFilesContentFromGoogle();
        }



    }
}
