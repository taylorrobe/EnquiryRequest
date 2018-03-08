using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EnquiryRequest3.Startup))]
namespace EnquiryRequest3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
