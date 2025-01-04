using Billing.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billing.Core.Extensions
{
    public static class BaseModelExtension
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
