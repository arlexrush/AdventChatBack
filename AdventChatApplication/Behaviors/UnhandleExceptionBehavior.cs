using MediatR;
using Microsoft.Extensions.Logging;

namespace AdventChatApplication.Behaviors
{
    public class UnhandleExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandleExceptionBehavior(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();

            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex, "Application Request: have happened an exception for the request: {Name} {@Request}", requestName, request);
                throw new Exception("Application Request has errors");
            }
        }
    }    
}
