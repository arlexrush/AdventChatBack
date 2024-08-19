using AdventChatApplication.Features.AdventChat.Commands.EmbeddingChunksCommands;
using AdventChatApplication.Features.AdventChat.Queries.GetChat;
using AdventChatApplication.Features.AdventChat.Vms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdventChatAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdventChatController:ControllerBase
    {
        private readonly IMediator? _mediator;

        public AdventChatController(IMediator? mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("sendPrompt", Name = "SendPrompt")]
        [ProducesResponseType(typeof(PromptVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PromptVm>> SendPrompt([FromBody] GetChatQuery request)
        {
            var response = await _mediator!.Send(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("sendChunkToEmbeddingStore", Name= "SendChunkToEmbeddingStore")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Unit>> ToEnbeddingChunks()
        {
            var command = new EmbeddingChunksCommand();
            var response= await _mediator!.Send(command);
            return Ok(response);

        }
    }
}
