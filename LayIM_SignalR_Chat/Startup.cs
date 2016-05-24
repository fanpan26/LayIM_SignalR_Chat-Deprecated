using LayIM.Queue;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LayIM_SignalR_Chat.Startup))]
namespace LayIM_SignalR_Chat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/layim", map =>
            {
                var hubConfiguration = new HubConfiguration()
                {
                    EnableJSONP = true
                };
                map.RunSignalR(hubConfiguration);
            });

            //开启队列监听
            ChatQueue.StartListeningChat();
        }
    }
}
