using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SmsMfaNotificationService.Api.Actions.Commands
{
    public static class SmsReceived
    {
        public record Command(string ClientId, object Payload) : IRequest<Result>;

        public record Result;

        public record SuccessfulResult : Result;

        public record FailedResult(string Error) : Result;

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger)
            {
                _logger = logger;
            }

            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Received from client '{request.ClientId}' '{request.Payload}'.");

                return Task.FromResult<Result>(new SuccessfulResult());
            }
        }
    }
}
