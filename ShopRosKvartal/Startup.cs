using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShopRosKvartal.Startup))]
namespace ShopRosKvartal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
