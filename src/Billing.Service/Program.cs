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
                            log.Info("Error: ���񽺸��� �Է��ϼ���. ��) -install MyService");
                        }
                        return;
                    case "-debug":
                        log.Info("����� ��� ���� ��...");
                        CreateHostBuilder(args).Build().Run();
                        return;

                    default:
                        log.Info("�� �� ���� ����Դϴ�.");
                        log.Info("����: -install {���񽺸�}, -debug");
                        return;
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            log.Info($"���� HostBuilder ����...");

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
            .ConfigureServices(ConfigureServices) // DI ���� �޼��� ȣ��
            .UseWindowsService(); // Windows ���� ��� Ȱ��ȭ

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

                    // ������ HTTP Ȯ�ο� ��������Ʈ
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
                log.Info($"���� '{serviceName}'�� ��ġ ��...");
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("���� ���� ��θ� ã�� �� �����ϴ�.");
                Process.Start("sc.exe", $"create {serviceName} binPath= \"{exePath}\" start= auto").WaitForExit();
                log.Info($"���� '{serviceName}' ��ġ �Ϸ�.");
            }
            catch (Exception ex)
            {
                log.Info($"���� ��ġ �� ���� �߻�: {ex.Message}");
            }
        }
    }
}