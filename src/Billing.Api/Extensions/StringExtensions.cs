using System.Text.Json;

namespace Billing.Api.Extensions
{
    public static class StringExtensions
    {
        public static Stream GetStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static bool IsValidJson(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input = input.Trim();
            if ((input.StartsWith('{') && input.EndsWith('}')) || // Object
                (input.StartsWith('[') && input.EndsWith(']')))  // Array
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(input);
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
