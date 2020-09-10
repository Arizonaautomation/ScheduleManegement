using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrainningManagement.Startup))]
namespace TrainningManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
