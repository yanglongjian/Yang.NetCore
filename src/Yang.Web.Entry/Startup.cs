using Microsoft.Extensions.Configuration;

namespace Yang.Web.Entry
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices()
        {
            // ����Ǩ���� WebStartup
        }
        public void Configure()
        {
            // ����Ǩ���� WebStartup
        }
    }
}
