using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
