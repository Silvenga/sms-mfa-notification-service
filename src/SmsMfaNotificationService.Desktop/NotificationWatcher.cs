using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NLog;
using SmsMfaNotificationService.Desktop.Models;

namespace SmsMfaNotificationService.Desktop
{
    public class NotificationWatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _clientId;
        private readonly HubConnection _connection;

        public event EventHandler<MfaCodeReceived>? MfaCodeReceived;

        public NotificationWatcher(string clientId, string hubHost)
        {
            _clientId = clientId;

            _connection = new HubConnectionBuilder()
                          .WithUrl(hubHost + "/hubs/notifications")
                          .WithAutomaticReconnect()
                          .Build();
            _connection.Reconnected += OnReconnected;
            _connection.Reconnecting += OnReconnecting;
            _connection.On<MfaCodeReceived>("ReceiveMfaCode", OnReceiveMfaCode);
        }

        public async Task Connect()
        {
            while (true)
            {
                Logger.Debug("Trying to open socket to notification hub.");
                try
                {
                    await _connection.StartAsync();
                    await OnReconnected(_connection.ConnectionId);

                    Logger.Debug("Connection open and subscriptions in-place.");
                    return;
                }
                catch (Exception e)
                {
                    Logger.Warn(e, "Failed while trying to connect to hub, trying again in 60 seconds.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        public async Task Shutdown()
        {
            Logger.Debug("Shutdown was called, gracefully disconnecting from hub.");
            await _connection.DisposeAsync();
        }

        private Task OnReconnecting(Exception arg)
        {
            Logger.Debug(arg, "An error occured, attempting to reconnect to hub.");
            return Task.CompletedTask;
        }

        private async Task OnReconnected(string arg)
        {
            Logger.Debug("Reconnected to hub after failure, adding subscriptions.");
            await _connection.SendAsync("SubscribeToClientId", _clientId);
        }

        private void OnReceiveMfaCode(MfaCodeReceived payload)
        {
            Logger.Debug("Got mfa code notification, notifing handlers.");
            OnMfaCodeReceived(payload);
        }

        protected virtual void OnMfaCodeReceived(MfaCodeReceived e)
        {
            MfaCodeReceived?.Invoke(this, e);
        }
    }
}
