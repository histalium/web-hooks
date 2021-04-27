using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebHooks.Utilities;

namespace WebHooks.Sender
{
    class Program
    {
        static async Task Main()
        {
            while (true)
            {
                Console.Write("message: ");
                var message = Console.ReadLine();

                Console.Write("key: ");
                var key = Console.ReadLine();

                var data = new HookData { Message = message };
                var json = JsonSerializer.Serialize(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                await Send("http://localhost:18080/hook", bytes, key);
            }
        }

        static async Task Send(string url, byte[] data, string key)
        {
            using var client = new HttpClient();
            using var content = new ByteArrayContent(data);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var signature = Signature.GenerateSignature(data, key);
            content.Headers.Add("X-Histalium-Signature-256", signature);
            var response = await client.PostAsync(url, content);
        }
    }
}
