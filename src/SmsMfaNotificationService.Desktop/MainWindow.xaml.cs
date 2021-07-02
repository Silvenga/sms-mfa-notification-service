using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Toolkit.Uwp.Notifications;
using SmsMfaNotificationService.Desktop.Models;
using TextCopy;

namespace SmsMfaNotificationService.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HubConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;

            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompatOnOnActivated;

            _connection = new HubConnectionBuilder()
                          .WithUrl("http://localhost:5000/hubs/notifications")
                          .WithAutomaticReconnect()
                          .Build();

            _connection.Reconnected += OnReconnected;

            _connection.On<MfaCodeReceived>("ReceiveMfaCode", OnReceiveMfaCode);

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await _connection.StartAsync();
                        await OnReconnected(_connection.ConnectionId);
                        return;
                    }
                    catch (Exception)
                    {
                        // 
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            });
        }

        private async Task OnReconnected(string arg)
        {
            await _connection.SendAsync("SubscribeToClientId", "testtest");
        }

        private void OnInitialized(object sender, EventArgs e)
        {
        }

        private void OnReceiveMfaCode(MfaCodeReceived payload)
        {
            Dispatcher.Invoke(() =>
            {
                new ToastContentBuilder()
                    .AddText("SMS MFA Code Detected", hintMaxLines: 1)
                    .AddText(payload.FullMessage)
                    .AddButton(new ToastButton()
                               .AddArgument("action", "copy")
                               .AddArgument("code", payload.Code)
                               .SetContent($"Copy '{payload.Code}'"))
                    .AddButton(new ToastButtonDismiss())
                    .SetToastScenario(ToastScenario.Reminder)
                    .AddAttributionText($"From {payload.SourcePhoneNumber}")
                    .Show(toast => { toast.ExpirationTime = DateTime.Now.AddMinutes(2); });
            });
        }

        private void ToastNotificationManagerCompatOnOnActivated(ToastNotificationActivatedEventArgsCompat toastArgs)
        {
            var args = ToastArguments.Parse(toastArgs.Argument);

            if (args.TryGetValue("action", out var action)
                && action == "copy"
                && args.TryGetValue("code", out var code))
            {
                ClipboardService.SetText(code);
            }
        }

        private void OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                Visibility = Visibility.Hidden;
            }
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            await _connection.DisposeAsync();
        }
    }
}
