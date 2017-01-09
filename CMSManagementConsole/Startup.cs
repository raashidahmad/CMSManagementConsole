using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMSManagementConsole.Startup))]
namespace CMSManagementConsole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
