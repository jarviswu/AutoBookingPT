using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AutoBooking2.Startup))]
namespace AutoBooking2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
