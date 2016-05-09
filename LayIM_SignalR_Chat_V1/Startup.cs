using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LayIM_SignalR_Chat_V1.Startup))]
namespace LayIM_SignalR_Chat_V1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
