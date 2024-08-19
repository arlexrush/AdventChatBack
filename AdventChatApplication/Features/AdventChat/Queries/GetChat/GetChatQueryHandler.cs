using AdventChatApplication.Contracts.Infrastructure;
using AdventChatApplication.Features.AdventChat.Vms;
using MediatR;

namespace AdventChatApplication.Features.AdventChat.Queries.GetChat
{
    public class GetChatQueryHandler : IRequestHandler<GetChatQuery, PromptVm>
    {
        private readonly IGenerativeResponseService? _generativeResponseService;

        public GetChatQueryHandler(IGenerativeResponseService? generativeResponseService)
        {
            _generativeResponseService = generativeResponseService;
        }

        public async Task<PromptVm> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            var contextPromt = await _generativeResponseService!.GetPromptFromContext(request.Prompt!);
            var responsePrompt=await _generativeResponseService.GetResponseFromOpenAI(contextPromt);
            var response=new PromptVm { Prompt = responsePrompt };
            return response;
        }
    }
}
