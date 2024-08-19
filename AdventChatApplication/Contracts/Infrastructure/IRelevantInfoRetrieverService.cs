using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventChatApplication.Contracts.Infrastructure
{
    public interface IRelevantInfoRetrieverService
    {
        public Task<string> GenerateResponseAsync(string query);
    }
}
