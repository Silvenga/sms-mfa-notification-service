using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SmsMfaNotificationService.Api.Helpers;
using SmsMfaNotificationService.Api.Hubs;
using SmsMfaNotificationService.Api.Parsing;

namespace SmsMfaNotificationService.Api.Actions.Commands
{
    public static class SmsReceived
    {
        public record Command(string ClientId, string PhoneNumber, string Message) : IRequest<Result>;

        public record Result;

        public record SuccessfulResult : Result;

        public record FailedResult(string Error) : Result;

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IHubContext<NotificationsHub, INotificationsClient> _notificationsHubContext;

            public Handler(ILogger<Handler> logger, IHubContext<NotificationsHub, INotificationsClient> notificationsHubContext)
            {
                _logger = logger;
                _notificationsHubContext = notificationsHubContext;
            }

            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogTrace($"Received from client '{request}'.");

                if (MessageParser.TryGetCode(request.Message, out var code))
                {
                    _logger.LogTrace($"Successfully parse code '{code}'.");
                    var groupName = NotificationsHubHelper.ClientIdToGroupName(request.ClientId);
                    _notificationsHubContext.Clients.Group(groupName).ReceiveMfaCode(new MfaCodeReceived(request.PhoneNumber, code, request.Message));
                }

                return Task.FromResult<Result>(new SuccessfulResult());
            }
        }
    }
}
