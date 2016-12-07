using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CrawlerMVC.Startup))]

namespace CrawlerMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}