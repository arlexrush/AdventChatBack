using AdventChatApplication.Features.AdventChat.Vms;
using MediatR;
using System.Net;

namespace AdventChatApplication.Features.AdventChat.Queries.GetChat
{
    public class GetChatQuery:IRequest<PromptVm>
    {
        public string? Prompt {  get; set; }
    }
}
