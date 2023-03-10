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
            // 代码迁移至 WebStartup
        }
        public void Configure()
        {
            // 代码迁移至 WebStartup
        }
    }
}
