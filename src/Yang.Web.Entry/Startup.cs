using Furion;
using Newtonsoft.Json;
using Yang.Web.Pack;
using Yang.Admin.Application;
namespace Yang.Web.Entry
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //初始化数据表
            DbSeedPack.InitTables();
            //初始化模块功能数据
            ModulePack.InitModule();
            AdminSeedData.InitData();

            //添加context请求
            services.AddHttpContextAccessor();
            //远程请求服务
            services.AddRemoteRequest();
            //添加缓存
            services.AddMemoryCache();
            services.AddConsoleFormatter();
            //安全鉴权
            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);

            services.AddCorsAccessor();
            services.AddControllers()
                .AddInjectWithUnifyResult()
                .AddNewtonsoftJson(options =>
                {
                    // 忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // 不使用驼峰 默认首字符小写
                    // options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // 设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    // 如字段为null值，该字段不会返回到前端
                    //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddControllersAsServices();

            //定时任务
            //builder.Services.AddSchedule();
            //EventBus事件服务
            services.AddEventBus(builder => { });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseCorsAccessor();
            app.UseAuthentication();
            app.UseAuthorization();
            // 添加规范化结果状态码
            app.UseUnifyResultStatusCodes();
            app.UseInject(string.Empty);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
