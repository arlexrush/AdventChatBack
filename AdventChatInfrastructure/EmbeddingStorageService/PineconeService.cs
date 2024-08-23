using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Models;
using AdventChatDomain;
using AdventChatInfrastructure.Models;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Pinecone;
using Pinecone.Grpc;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Text;
using System.Text.Json;
using Vector = Pinecone.Vector;

namespace AdventChatInfrastructure.EmbeddingStorageServices
{
    public class PineconeService : IPineconeService
    {
        private readonly Index<GrpcTransport>? _index;
        private readonly PineconeSettings? _settings;

        public PineconeService(IOptions<PineconeSettings>? settings, Index<GrpcTransport> index)
        {
           _settings = settings!.Value;
            _index = index ?? throw new ArgumentNullException();

        }

        
        // Inserción y Actualización de Vectores
        public async Task<bool> UpsertVectorsAsync(List<DocumentRagWithEmbedding> vectors)
        {
            var vectorsEmbedding = new List<Vector>();
            
            foreach (var vector in vectors)
            {
                var vectorEmbedding = new Vector()
                {
                    Id = vector.Id,
                    Values = vector.Embedding!,
                    Metadata = new MetadataMap(vector.Document!.Metadata!.Select(x=> new KeyValuePair<string, MetadataValue>(x.Key, x.Value)))
                };
                vectorsEmbedding.Add(vectorEmbedding);
            }
            try
            {
                await _index!.Upsert(vectorsEmbedding);
                return true;
            }catch (Exception ex)
            {
                throw;
            }
            
        }


        //buscar los vectores más similares a un vector de consulta dado en tu índice de Pinecone. topK= Numero de vectores que se quieren en la respuesta
        public async Task<List<PineconeQueryResult>> QueryAsync(float[] queryVector, uint topK)
        {   
            var resultArray =await _index!.Query(queryVector, topK);
            var resultList=resultArray.ToList();
            var dictionary = new Dictionary<string, object>();
            var metadataExtract = resultList.Select(x=>x.Metadata).ToList();
            var metadataFit= metadataExtract.Select(x => { x.Select(item => dictionary[item.Key] = item.Value.ToString()); return dictionary; }).ToList();
            var metadataX = metadataFit[0];
            var response = resultList.Select(result => new PineconeQueryResult(result.Id, result.Score, result!.Values!, metadataX)).ToList();

            return response;
        }

        // Borrar vectores por su id
        public async Task<bool> DeleteVectorsAsync(List<string> ids)
        {
            IEnumerable<string> idN = ids;
            try
            {
                await _index!.Delete(idN);
                return true;
            }catch (Exception ex)
            {
                throw;
            }
            
        }

        // Sirve para recuperar los metadatos de un vector dado con un id como parametro.
        public async Task<Dictionary<string, object>> FetchVectorAsync(string id)
        {            
            var result = await _index!.Fetch(new List<string>() { id });
            var response=result.ToDictionary(item=>item.Key, item=>(object)item.Value);
            return response;
        }
    }
}
