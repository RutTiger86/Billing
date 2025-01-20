using Billing.Core.Interfaces;
using Billing.Core.Services;
using Billing.Service.Services;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Billing.Service
{
    public class Program
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            var log4netConfigPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            XmlConfigurator.Configure(new FileInfo(log4netConfigPath));

            if (args.Length > 0)
            {
                string command = args[0].ToLower();

                switch (command)
                {
                    case "-install":
                        if (args.Length > 1)
                        {
                            string serviceName = args[1];
                            InstallService(serviceName);
                        }
                        else
                        {
                            log.Info("Error: 서비스명을 입력하세요. 예) -install MyService");
                        }
                        return;
                    case "-debug":
                        log.Info("디버그 모드 실행 중...");
                        CreateHostBuilder(args).Build().Run();
                        return;

                    default:
                        log.Info("알 수 없는 명령입니다.");
                        log.Info("사용법: -install {서비스명}, -debug");
                        return;
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            log.Info($"서비스 HostBuilder 생성...");

            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddLog4Net("log4net.config");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                ConfigureKestrel(webBuilder);
                ConfigureGrpcServices(webBuilder);
                ConfigureGrpcPipeline(webBuilder);
            })
            .ConfigureServices(ConfigureServices) // DI 구성 메서드 호출
            .UseWindowsService(); // Windows 서비스 모드 활성화

            return builder;
        }

        private static void ConfigureGrpcServices(IWebHostBuilder webBuilder)
        {
            webBuilder.ConfigureServices(services =>
            {
                services.AddGrpc();
            });
        }

        private static void ConfigureKestrel(IWebHostBuilder webBuilder)
        {
            webBuilder.ConfigureKestrel((context, options) =>
            {
                options.Configure(context.Configuration.GetSection("Kestrel"));
            });

        }

        private static void ConfigureGrpcPipeline(IWebHostBuilder webBuilder)
        {
            webBuilder.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<BillGrpcServiceImpl>();
                    endpoints.MapGrpcService<TransactionGrpcServiceImpl>();

                    // 간단한 HTTP 확인용 엔드포인트
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("gRPC server is running. Use a gRPC client to communicate.");
                    });
                });
            });
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddScoped<IBillTxService, BillTxService>();
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<BillGrpcServiceImpl>();
            services.AddScoped<TransactionGrpcServiceImpl>();
        }

        private static void InstallService(string serviceName)
        {
            try
            {
                log.Info($"서비스 '{serviceName}'를 설치 중...");
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("실행 파일 경로를 찾을 수 없습니다.");
                Process.Start("sc.exe", $"create {serviceName} binPath= \"{exePath}\" start= auto").WaitForExit();
                log.Info($"서비스 '{serviceName}' 설치 완료.");
            }
            catch (Exception ex)
            {
                log.Info($"서비스 설치 중 오류 발생: {ex.Message}");
            }
        }
    }
}