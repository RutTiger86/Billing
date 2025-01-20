using Billing.Grpc.TestClient.Services;
using Billing.Grpc.TestClient.Views;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Billing.Grpc.TestClient
{
    public class MainWindowModel: BaseModel
    {
        private readonly IServiceProvider serviceProvider;

        private Page currentView;
        public Page CurrentView
        {
            get => currentView;
            set => SetProperty(ref currentView, value);
        }

        public MainWindowModel(IServiceProvider serviceProvider)
        {
            LogInfo("★★★★★ MainWindowModel Start ★★★★★");
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            SetView(typeof(ConnectView));
        }

        private void SetView(Type type)
        {
            CurrentView = (Page)serviceProvider.GetRequiredService(type);
        }
    }
}
