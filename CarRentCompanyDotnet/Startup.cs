using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarRentCompanyDotnet.Startup))]
namespace CarRentCompanyDotnet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
