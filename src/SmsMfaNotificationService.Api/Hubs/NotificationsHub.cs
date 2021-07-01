using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SmsMfaNotificationService.Api.Helpers;

namespace SmsMfaNotificationService.Api.Hubs
{
    public interface INotificationsClient
    {
        Task ReceiveMfaCode(MfaCodeReceived payload);
    }

    public class NotificationsHub : Hub<INotificationsClient>
    {
        private readonly ILogger<NotificationsHub> _logger;

        public NotificationsHub(ILogger<NotificationsHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client '{Context.ConnectionId}' connected to this hub.");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation(exception, $"Client '{Context.ConnectionId}' disconnected from this hub.");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeToClientId(string clientId)
        {
            var groupName = NotificationsHubHelper.ClientIdToGroupName(clientId);
            _logger.LogInformation($"Client '{Context.ConnectionId}' requested subscription to group '{groupName}'.");
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
