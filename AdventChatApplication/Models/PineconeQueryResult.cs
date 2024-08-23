using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventChatApplication.Models
{
    public class PineconeQueryResult
    {
        public string Id { get; set; }
        public double Score { get; set; }
        public float[] Values { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public PineconeQueryResult(string id, double score, float[] values, Dictionary<string, object> metadata)
        {
            Id = id;
            Score = score;
            Values = values;
            Metadata = metadata;
        }

        // Podríamos añadir métodos útiles aquí, por ejemplo:
        public bool TryGetMetadata<T>(string key, out T value)
        {
            if (Metadata.TryGetValue(key, out var obj) && obj is T typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default!;
            return false;
        }
    }
}
