

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DeclarativeSharp.Services;
using Newtonsoft.Json;

namespace DeclarativeSharp {
    public class NetworkUtil {
        public async Task<bool> VerifyReceipt(AppleReceipt receipt) {
            using var client = new HttpClient(new HttpClientHandler());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var jsonStr = JsonConvert.SerializeObject(receipt);

            var result = await client.PostAsync(
                new Uri("http://192.168.1.177:5134/Receipt"),
                new StringContent(jsonStr, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode) {
//                throw new Exception("Remote receipt verification error");
                Console.WriteLine($"code: {result.StatusCode}");
            }

            return true;
        }
    }
}
