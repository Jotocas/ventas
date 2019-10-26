using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ventas.Backend.Startup))]
namespace Ventas.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
