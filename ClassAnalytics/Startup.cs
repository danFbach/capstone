using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClassAnalytics.Startup))]
namespace ClassAnalytics
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
