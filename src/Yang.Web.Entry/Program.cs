using AspectCore.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Web.Entry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

        #region ���Զ�ȡ�����ļ�
                //����vscode�������ö�ȡ�������� todo:�޸�launch.json cwd ·��
                //.ConfigureAppConfiguration((hostContext, config) =>
                //{
                //    var env = hostContext.HostingEnvironment;
                //    config.SetBasePath(Path.Combine(env.ContentRootPath, ""))
                //                            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);
                //})
        #endregion

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Inject().UseStartup<Startup>()

                    #region ����API��ʱʱ��
                    .UseKestrel(option =>
                    {
                        option.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(5);
                        option.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(5);
                    })

                    #endregion


                    #region ��־���
                    .UseSerilogDefault(config =>
                    {
                        string outputTemplate = "{Level:u3} {Timestamp:HH:mm:ss}  {Message:lj} {Properties:j}{NewLine}{Exception}";
                        //string outputTemplate = "{Level:u3} {Timestamp:HH:mm:ss}  {Message:lj} {NewLine}{Exception}";
                        LogEventLevel[] levels = new LogEventLevel[] {
                             LogEventLevel.Information,
                             LogEventLevel.Warning,
                             LogEventLevel.Error,
                             LogEventLevel.Fatal
                         };
                        config.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console(outputTemplate: outputTemplate);
                        foreach (var level in levels)
                        {
                            config.WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(evt => evt.Level == level)
                                   .WriteTo.File($"log/{DateTime.Now:yyyy-MM-dd}/{level}.log",
                                       outputTemplate: outputTemplate,
                                       rollingInterval: RollingInterval.Day,
                                       encoding: Encoding.UTF8
                                    )
                                );
                        }

                    });

                    #endregion


                }).UseDynamicProxy(); //��AspectCore�滻Ĭ�ϵ�IOC���� AOP���� AddControllersAsServices
    }
}