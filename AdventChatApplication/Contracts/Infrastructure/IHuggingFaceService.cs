using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IHuggingFaceService
    {
        public Task<HttpResponseMessage> SendRequestEnbeddingToHFAsync(string content, CancellationToken cancellationToken = default);
    }
}
