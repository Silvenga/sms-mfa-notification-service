using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SmsMfaNotificationService.Api.Hubs
{
    public interface INotificationsClient
    {
        Task ReceiveMessage(string message);
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

        public async Task SendMessage(string clientId, string message)
        {
            var groupName = GetGroup(clientId);
            await Clients.Group(groupName).ReceiveMessage(message);
        }

        public async Task SubscribeToClientId(string clientId)
        {
            var groupName = GetGroup(clientId);
            _logger.LogInformation($"Client '{Context.ConnectionId}' requested subscription to group '{groupName}'.");
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        private static string GetGroup(string clientId)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(clientId));
            var groupName = Convert.ToBase64String(hash);
            return groupName;
        }
    }
}
