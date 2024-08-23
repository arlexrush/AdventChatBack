using AdventChatApplication.Contracts.Infrastructure;
using AdventChatInfrastructure.ChunkingServices;
using AdventChatInfrastructure.CohereServices;
using AdventChatInfrastructure.EmbeddingServices;
using AdventChatInfrastructure.EmbeddingStorageServices;
using AdventChatInfrastructure.GenerativeResponseServices;
using AdventChatInfrastructure.GoogleFirebaseServices;
using AdventChatInfrastructure.HuggingFaceServices;
using AdventChatInfrastructure.Models;
using AdventChatInfrastructure.OpenAIServices;
using AdventChatInfrastructure.ProcessingDataServices;
using AdventChatInfrastructure.RelevantInfoRetrieverServices;
using AdventChatInfrastructure.SentenceTransformerServices;
using AdventChatInfrastructure.TypeChunkingServices;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Pinecone;
using Pinecone.Grpc;

namespace AdventChatInfrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<FirebaseSettings>(option =>
            //{
            //    var section = configuration.GetSection("FirebaseSettings");
            //});

            //Google FireBaseServices
            // Registra StorageClient como singleton
            services.Configure<FirebaseSettings>(configuration.GetSection("FirebaseSettings"));
            var section = services.Configure<OpenAISettings>(configuration.GetSection("OpenAISettings"));

            services.AddSingleton(serviceProvider =>
            {
                var firebaseSettings = serviceProvider.GetRequiredService<IOptions<FirebaseSettings>>().Value;
                var jsonContent = new
                {
                    type = firebaseSettings.type,
                    project_id = firebaseSettings.project_id,
                    private_key_id = firebaseSettings.private_key_id,
                    private_key = firebaseSettings.private_key!.Replace("\\n", "\n"),
                    client_email = firebaseSettings.client_email,
                    client_id = firebaseSettings.client_id,
                    auth_uri = firebaseSettings.auth_uri,
                    token_uri = firebaseSettings.token_uri,
                    auth_provider_x509_cert_url = firebaseSettings.auth_provider_x509_cert_url,
                    client_x509_cert_url = firebaseSettings.client_x509_cert_url,
                    universe_domain = firebaseSettings.universe_domain
                };
                var json = JsonSerializer.Serialize(jsonContent);
                var credential = GoogleCredential.FromJson(json);
                return StorageClient.Create(credential);
            });

            // Registro de Firebase Storage Settings
            services.Configure<FirebaseStorageSettings>(option =>
            {
                var section = configuration.GetSection("FirebaseStorageSettings");
                option.BucketName = section["BucketName"];
            });

            // Registro de HFSettings 
            services.Configure<HFSettings>(configuration.GetSection("HFSettings"));
            services.Configure<HFSettings>(option =>
            {
                var section = configuration.GetSection("HFSettings");
            });

            // Registro de CohereSettings 
            services.Configure<CohereSettings>(configuration.GetSection("CohereSettings"));
            services.Configure<CohereSettings>(option =>
            {
                var section = configuration.GetSection("HFSettings");
            });

            services.AddHttpClient();


            // Sentence Transformers Settings Registration
            services.Configure<SentenceTransformerSettings>(configuration.GetSection("SentenceTransformerSettings"));
            services.Configure<SentenceTransformerSettings>(option =>
            {
                var section = configuration.GetSection("HFSettings");
            });


            // Google Firebase registration
            services.AddScoped<IGoogleFirebaseService, GoogleFirebaseService>();

            //Processing Data Service Registration
            services.AddScoped<IProcessingDataService, ProcessingDataService>();

            //Chunking Service Registration
            services.Configure<ChunkingSettings>(option =>
            {
                var section = configuration.GetSection("ChunkingSettings");
            }
            );
            services.AddScoped<IChunkingService, ChunkingService>();

            //Type Chunking Services Registration
            services.AddScoped<ITypeChunkingService, TypeChunkingService>();

            //Embedding Services Registration
            services.AddScoped<IEmbeddingService, EmbeddingService>();

            // Embedding Hugging Face Service Registration
            services.AddScoped<IHuggingFaceService, HuggingFaceService>();

            // Sentence Transformers Service Registration
            services.AddScoped<ISentenceTransformerService, SentenceTransformerService>();

            // Embedding Cohere Service Registration
            services.AddScoped<ICohereService, CohereService>();

            //OpenAI Service Registration            
            services.AddScoped<IOpenAIService, OpenAIService>();

            //Pinecone Service Registration
            services.Configure<PineconeSettings>(configuration.GetSection("PineconeSettings"));            
            services.AddScoped<IPineconeService, PineconeService>();
            services.AddSingleton(serviceProvider => {
                var apikey = serviceProvider.GetRequiredService<IOptions<PineconeSettings>>().Value.ApiKey;
                var nameIndex= serviceProvider.GetRequiredService<IOptions<PineconeSettings>>().Value.IndexName;
                var client = new PineconeClient(apikey!);
                var index=client.GetIndex(nameIndex!).Result;
                return index;
            });

            //Embedding Store Service
            services.AddScoped<IEmbeddingsStoreService, EmbeddingsStoreService>();

            //Relevant Info Retriever Service
            services.AddScoped<IRelevantInfoRetrieverService, RelevantInfoRetrieverService>();

            //Generative Response Service
            services.AddScoped<IGenerativeResponseService, GenerativeResponseService>();


            return services;
        }
    }
}
