using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Grpc.TestClient.Services
{
    public interface IBillingGrpcSerivce
    {
        Task<bool> SetService(string address);
    }

    public class BillingGrpcService : IBillingGrpcSerivce
    {
        private GrpcChannel? channel;
        public async Task<bool> SetService(string address)
        {
            channel = GrpcChannel.ForAddress(address);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            try
            {
                // 연결 시도 (타임아웃 적용)
                await channel.ConnectAsync(cts.Token);
                Console.WriteLine($"Channel state after ConnectAsync: {channel.State}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Connection attempt timed out.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
            finally
            {
                await channel.ShutdownAsync();                
            }
            return true;
        }
    }
}
