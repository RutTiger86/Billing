using Billing.Api.Extensions;
using Billing.Api.Models.Respons;
using Billing.Core.Enums;
using System.Diagnostics;
using System.Text.Json;

namespace Billing.Api.Middlewares
{

    public class LoggingMiddleware(ILogger<LoggingMiddleware> logger, RequestDelegate next, IConfiguration configuration)
    {
        private readonly ILogger<LoggingMiddleware> logger = logger;
        private readonly RequestDelegate next = next;

        private IConfiguration configuration = configuration;
        private readonly HashSet<string> notloggingProperties = ["notLoggingData", "secretData"];

        public async Task InvokeAsync(HttpContext context)
        {
            Stream resultResponseBody = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();

                Stopwatch stopwatch = Stopwatch.StartNew();

                string logMessage = await context.GetRequestLogMessageAsync(notloggingProperties);

                logger.LogInformation(logMessage);

                context.Response.Body = memoryStream;

                await next(context);

                memoryStream.Position = 0;
                string responseBody = new StreamReader(memoryStream).ReadToEnd();

                memoryStream.Position = 0;

                await memoryStream.CopyToAsync(resultResponseBody);

                stopwatch.Stop();

                int maxLoggingLength = configuration.GetValue<int>("Settings:MaxLoggingLength");
                logMessage = string.Empty;
                if (responseBody.Length > maxLoggingLength)
                {
                    int defaultDataEndindex = responseBody.IndexOf(",\"data\"");
                    if (defaultDataEndindex >= 0)
                    {
                        string defaultData = responseBody.Substring(0, defaultDataEndindex);
                        logMessage = context.GetLogMessage($"working time(ms) : {stopwatch.ElapsedMilliseconds}, defaultData : {defaultData}, response length {responseBody.Length}");
                    }
                    else
                    {
                        logMessage = context.GetLogMessage($"working time(ms) : {stopwatch.ElapsedMilliseconds}, response length {responseBody.Length}");
                    }
                }
                else
                {
                    logMessage = context.GetLogMessage($"working time(ms) : {stopwatch.ElapsedMilliseconds}, response data {responseBody}");
                }

                logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                string logMessage = context.GetLogMessage(ex.ToString());
                logger.LogError(ex, logMessage);

                BaseResponse<string> res = new()
                {
                    ErrorCode = (int)BillingError.SYSTEM_ERROR,
                    ErrorMessage = $"{BillingError.SYSTEM_ERROR}",
                    Data = ex.ToString(),
                    Result = false
                };

                var result = JsonSerializer.Serialize(res);

                await result.GetStream().CopyToAsync(resultResponseBody);
            }

        }
    }
}
