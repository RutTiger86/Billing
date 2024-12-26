using Billing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Billing.Core.Extensions
{
    public static class BaseModelExtensions
    {
        public static T DeepCopy<T>(this BaseModel obj)
        {
            var serialized = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }
}
