using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(SignalRServer.Startup))]
namespace SignalRServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR().MapSignalR();
            //app.azureSignalR();
            //app.MapAzureSignalR();
        }
    }
}