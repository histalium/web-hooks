using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebHooks.Utilities;

namespace WebHooks.Client
{
    public class WebHookHandler
    {
        private readonly ILogger<WebHookHandler> _logger;

        public WebHookHandler(ILogger<WebHookHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(HttpRequest request, HttpResponse response)
        {
            var bytes = await ReadAllBytes(request.Body, request.ContentLength.Value);

            var storedSignature = request.Headers["X-Histalium-Signature-256"].ToString();
            var calculatedSignature = Signature.GenerateSignature(bytes, "test");

            var err = !calculatedSignature.SequenceEqual(storedSignature);

            var json = Encoding.UTF8.GetString(bytes);
            var data = JsonSerializer.Deserialize<HookData>(json);

            if (err)
            {
                _logger.LogError("signature invalid");
                response.StatusCode = 400;
            }
            else
            {
                _logger.LogInformation("signature valid");
                _logger.LogInformation(data.Message);
                response.StatusCode = 200;
            }
        }

        private static async Task<byte[]> ReadAllBytes(Stream stream, long length)
        {
            var b = new byte[length];
            await stream.ReadAsync(b.AsMemory(0, b.Length));

            return b;
        }
    }
}
