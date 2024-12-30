using Billing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Billing.Core.Extensions
{
    public static class BaseModelExtensions
    {
        public static T DeepCopy<T>(this BaseModel obj)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }, // 열거형을 문자열로 직렬화
                WriteIndented = true
            };

            var serialized = JsonSerializer.Serialize((object)obj, obj.GetType(), options);
            return JsonSerializer.Deserialize<T>(serialized, options);
        }
    }
}
