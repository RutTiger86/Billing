using CommunityToolkit.Mvvm.ComponentModel;
using log4net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Billing.Grpc.TestClient
{

    public class BaseModel : ObservableObject, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool baseDisposed = false;

        ~BaseModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!baseDisposed)
            {
                if (disposing)
                {

                }
                // Note disposing has been done.
                baseDisposed = true;
            }
        }

        public void LogInfo(string Message, [CallerMemberName] string propertyName = "")
        {
            log.Info($"[{propertyName}]  {Message} ");
        }

        public void LogError(string Message, [CallerMemberName] string propertyName = "")
        {
            log.Error($"[{propertyName}]★{Message} ");
        }

        public void LogException(string Message, [CallerMemberName] string propertyName = "")
        {
            log.Fatal($"[{propertyName}]★★★{Message} ");
        }

    }
}
