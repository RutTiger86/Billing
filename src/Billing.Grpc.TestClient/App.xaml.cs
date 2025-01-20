using Billing.Grpc.TestClient.Services;
using Billing.Grpc.TestClient.ViewModels;
using Billing.Grpc.TestClient.Views;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Windows;

namespace Billing.Grpc.TestClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHost host;

        public App()
        {
            host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                context.HostingEnvironment.ApplicationName = "Billing.Grpc.TestClient";
                ConfigureServices(services);
            }).Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // ViewModels
            services.AddSingleton<MainWindowModel>();
            services.AddScoped<ConnectViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddScoped<ConnectView>();

            // Services
            services.AddSingleton<IBillingGrpcSerivce, BillingGrpcService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SettingMessage();
            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await host.StopAsync();
            base.OnExit(e);
        }

        private void SettingMessage()
        {
            try
            {

            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }

        private void ProgramShutdownProcess()
        {
            try
            {
                log.Info("ProgramShutdownProcess");
                host.Services.GetRequiredService<MainWindow>().Close();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }
    }

}
