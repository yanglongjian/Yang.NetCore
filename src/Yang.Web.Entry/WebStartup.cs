using CSRedis;
using Furion;
using Furion.DataEncryption.Extensions;
using Furion.Logging.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
//using PuppeteerSharp;
using System;
using Yang.Core;
using Yang.Web.Pack;

namespace Yang.Web.Entry
{
    /// <summary>
    /// Web服务注册启动项
    /// </summary>
    public class WebStartup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //添加context请求
            services.AddHttpContextAccessor();
            //远程请求服务
            services.AddRemoteRequest();
            //添加缓存
            services.AddMemoryCache();
            //安全鉴权
            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);

            //添加跨域服务
            services.AddCorsAccessor();


            services.AddControllers()
                  .AddInjectWithUnifyResult() //统一返回值(不设置swagger异常)
                //.AddMvcFilter<RequestHandler>()    //添加过滤器
                .AddNewtonsoftJson(options =>
                {
                    // 忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // 不使用驼峰
                    // options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // 设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    // 如字段为null值，该字段不会返回到前端
                    //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).AddControllersAsServices();//拦截器只会拦截从服务注册的接口，所以AddControllersAsServices()必须添加

            //启用运行时编译
            //services.AddRazorPages().AddRazorRuntimeCompilation();
            //添加视图引擎
            //services.AddViewEngine();



            // 添加即时通讯,支持MessagePack
            // services.AddSignalR().AddMessagePackProtocol();

            //初始化redis
            if (App.Configuration["ConnectionConfig:IsUseRedis"].ToBool())
            {
                RedisHelper.Initialization(new CSRedisClient(App.Configuration["ConnectionConfig:Redis"]));
            }
            //添加计划任务
            services.AddTaskScheduler();
            // 注册 EventBus 服务
            services.AddEventBus(builder => { });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/notfound");

            //app.UseMiddleware<HostRedirectMiddleware>();
            //app.UseHttpsRedirection();  //使用https

            //扫描不到No operations defined in spec 异常手动注册
            app.UseSpecificationDocuments();

            //添加跨域 中间件(需在 app.UseRouting(); 和 app.UseAuthentication(); 之间注册)
            app.UseCorsAccessor();
            app.UseStaticFiles();

            app.UseRouting();
            //添加授权服务 中间件
            app.UseAuthentication();
            app.UseAuthorization();
            // 添加规范化结果状态码，需要在这里注册 401 和 403、404 
            app.UseUnifyResultStatusCodes();
            app.UseInject(string.Empty);
            app.UseEndpoints(endpoints =>
            {
                // 注册集线器
                endpoints.MapControllers();
                //注册Razor
                endpoints.MapRazorPages();
            });
            //codefirst 数据表
            DbSeedPack.InitTables();
            //初始化模块功能数据
            ModulePack.InitModule();

            //new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            //"下载chrome浏览器完成".LogInformation();
        }
    }
}




