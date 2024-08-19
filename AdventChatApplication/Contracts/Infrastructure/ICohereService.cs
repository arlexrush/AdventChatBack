using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface ICohereService
    {
        public Task<HttpResponseMessage> SendRequestEnbeddingToCohereAsync(string content, CancellationToken cancellationToken = default);
    }
}
