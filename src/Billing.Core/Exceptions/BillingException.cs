using Billing.Core.Enums;

namespace Billing.Core.Exceptions
{
    public class BillingException : Exception
    {
        public BillingError ErrorCode { get; }

        public BillingException(BillingError errorCode, string message)
       : base(message)
        {
            ErrorCode = errorCode;
        }

        public BillingException(BillingError errorCode, string message, Exception innerException)
        : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return $"Error Code: {ErrorCode}, Message: {Message}";
        }
    }   
}
