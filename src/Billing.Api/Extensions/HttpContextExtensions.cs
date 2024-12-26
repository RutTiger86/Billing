using System.Diagnostics;
using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Billing.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetLogMessage(this HttpContext context, params string[] messages)
        {
            var resultMessage = $"API : {context.Request.Path} / ActivityID :{Activity.Current?.Id} / IP : {context.GetIp()} / ";

            for (int index = 0; index < messages.Length; index++)
            {
                resultMessage += $"{messages[index]} / ";
            }

            return resultMessage;
        }

        public async static Task<string> GetRequestLogMessageAsync(this HttpContext context, HashSet<string> notloggingProperties)
        {
            var request = context.Request;
            request.EnableBuffering();
            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            var apiParam = request.QueryString.ToString();

            if (notloggingProperties != null)
            {
                requestBody = RemoveSensitiveData(requestBody, notloggingProperties);
            }

            string resultMessage = context.GetLogMessage($"Request : {requestBody}{apiParam}");
            return resultMessage;
        }

        public static string? GetIp(this HttpContext context)
        {
            IPAddress? ipAddress = context?.Connection?.RemoteIpAddress;

            if (ipAddress?.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                ipAddress = Dns.GetHostEntry(ipAddress).AddressList.First(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            return ipAddress?.ToString();
        }

        private static string? RemoveSensitiveData(string requestBody, HashSet<string> notloggingProperties)
        {
            if (!requestBody.IsValidJson()) return requestBody;

            JsonObject? jsonObject = JsonNode.Parse(requestBody)?.AsObject();

            if (jsonObject != null)
            {
                foreach (var property in notloggingProperties)
                {
                    if (jsonObject.ContainsKey(property))
                    {
                        jsonObject[property] = JsonValue.Create("*****");
                    }
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            return jsonObject?.ToJsonString(options);
        }
    }
}
