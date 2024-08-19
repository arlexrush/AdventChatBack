using AdventChatApplication.Models;
using AdventChatInfrastructure.Models;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace AdventChatInfrastructure.GoogleFirebaseServices
{
    public class GoogleFirebaseService: IGoogleFirebaseService
    {
        private readonly StorageClient? _storageClient;
        private readonly FirebaseStorageSettings _storageSettings;
        private readonly string? _bucketName;

        public GoogleFirebaseService(IOptions<FirebaseStorageSettings> firebaseStorageSettigs, StorageClient storageClient)
        {
            _storageClient = storageClient;
            _storageSettings = firebaseStorageSettigs.Value;
            _bucketName = firebaseStorageSettigs.Value.BucketName;
        }

        public async Task<List<FileRag>> GetAllFilesContentFromGoogle()
        {
            var filerag = new FileRag();
            var filesRag = new List<FileRag>();
            var storageObjects = _storageClient!.ListObjects(_bucketName);
            var listObjectFiltered = storageObjects.Where(x => x.Name.Equals("tradbibl.txt")).ToList();

            foreach (var storageObject in listObjectFiltered)
            {
                if (storageObject.Name.EndsWith(".txt"))
                {
                    filerag.FileName = storageObject.Name;
                    var stream = new MemoryStream();
                    await _storageClient.DownloadObjectAsync(_bucketName, storageObject.Name, stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();
                        filerag.FileContent = content;
                    }
                    filesRag.Add(filerag);
                }
            }

            return filesRag;
        }

    }
}
