using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Seo.Prerender.SampleSite.Startup))]
namespace Seo.Prerender.SampleSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
