using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsMfaNotificationService.Api.Formatters.Tasker;
using SmsMfaNotificationService.Api.Hubs;

namespace SmsMfaNotificationService.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => o.InputFormatters.Insert(o.InputFormatters.Count, new TaskerSmsInputFormatter()));
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Startup>());
            services.AddSignalR();
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationsHub>("/hubs/notifications");
            });

            app.UseHealthChecks("/api/v1/health");
        }
    }
}
