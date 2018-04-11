using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StockWebApplication45.Startup))]
namespace StockWebApplication45
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
