using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatInfrastructure.SentenceTransformerServices
{
    public interface ISentenceTransformerService
    {
        public Task<HttpResponseMessage> SendRequestEnbeddingToSTModelAsync(string content, CancellationToken cancellationToken = default);
    }
}
