using Billing.Grpc.TestClient.Services;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Billing.Grpc.TestClient.ViewModels
{  

    public class ConnectViewModel:BaseModel
    {
        private readonly IBillingGrpcSerivce billingGrpcSerivce;

        #region Binding Value

        private string grpcAddress;
        public string GrpcAddress
        {
            get => grpcAddress;
            set => SetProperty(ref grpcAddress, value);
        }

        private bool isEnableView;
        public bool IsEnableView
        {
            get => isEnableView;
            set => SetProperty(ref isEnableView, value);
        }
        #endregion

        #region Command
        public RelayCommand GrpcSetCommand
        {
            get;
            private set;
        }
        #endregion

        public ConnectViewModel(IBillingGrpcSerivce billingGrpcSerivce)
        {
            this.billingGrpcSerivce = billingGrpcSerivce ?? throw new ArgumentNullException(nameof(billingGrpcSerivce));
            SettingCommand();
            IsEnableView = true;
        }
        private void SettingCommand()
        {
            try
            {
                GrpcSetCommand = new RelayCommand(BillingGrpcSerivceSet);
            }
            catch (Exception ex)
            {
                LogException(ex.Message);
            }

        }

        private async void BillingGrpcSerivceSet()
        {
            IsEnableView = false;

            try
            {
                if (await billingGrpcSerivce.SetService(GrpcAddress))
                {
                    Console.WriteLine("GrpcServiceSet Success");
                }
                else
                {
                    Console.WriteLine("GrpcServiceSet Faild");
                }
            }
            catch (Exception ex)
            {
                LogException(ex.Message);
            }
            finally
            {
                IsEnableView = true;
            }
        }
    }
}
