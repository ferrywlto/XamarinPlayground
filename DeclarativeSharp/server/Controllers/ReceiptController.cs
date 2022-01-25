using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ReceiptController : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> Post(AppleReceipt receipt) {
            var base64Str = Convert.ToBase64String(receipt.Data);
            Console.WriteLine(receipt.TransactionId);

            var result = await VerifyReceiptOnApple(base64Str, receipt.Id);

            if (result) return Ok();

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Test() {
            const string receipt
                = "ewoJInNpZ25hdHVyZSIgPSAiQXorTGdka3BIWEtWUXZuSmtBWm4wV0U3MWJMeVYrbW54UzhlbEVCZlVUYTRjNU9HQXJuZ0JhdjVTc05EdVJoL2E2YTdITFQ0OGpTcDg1dWRYdjRHWWY1cUR5NWJRYVF0WC9RbHpld1IrWDk3c0pyYlIybTE2bkRSS1pwMFRmSGg5a0h0Zy9oYnpaYVFQNG1vNDVZZzBGQ20vVmJCQi9YU01tWkMwWFpFeTBJejg4cDc3WW9LYUVPUHo5SmpweFhjc0VaN1I3VG1vdDdKZlg4S2dLQkVjVTZDK0w5VTc1eDBNcW8rWTN3MHZUL3JSVlJpYzR4ZjdzRGp0ek10S1BYRW1qdUJNWTcvMlZZZU80ZVF4TTNBNHlQaE8xUHBldEY5R3JWMDBaakc4emdnS0ZockN1TkhTMzBDSVhKSnBWVGIxSVNSRjR1TGxxOTlBZ1FYUjN6M3B4SUFBQVdBTUlJRmZEQ0NCR1NnQXdJQkFnSUlEdXRYaCtlZUNZMHdEUVlKS29aSWh2Y05BUUVGQlFBd2daWXhDekFKQmdOVkJBWVRBbFZUTVJNd0VRWURWUVFLREFwQmNIQnNaU0JKYm1NdU1Td3dLZ1lEVlFRTERDTkJjSEJzWlNCWGIzSnNaSGRwWkdVZ1JHVjJaV3h2Y0dWeUlGSmxiR0YwYVc5dWN6RkVNRUlHQTFVRUF3dzdRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTWdRMlZ5ZEdsbWFXTmhkR2x2YmlCQmRYUm9iM0pwZEhrd0hoY05NVFV4TVRFek1ESXhOVEE1V2hjTk1qTXdNakEzTWpFME9EUTNXakNCaVRFM01EVUdBMVVFQXd3dVRXRmpJRUZ3Y0NCVGRHOXlaU0JoYm1RZ2FWUjFibVZ6SUZOMGIzSmxJRkpsWTJWcGNIUWdVMmxuYm1sdVp6RXNNQ29HQTFVRUN3d2pRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTXhFekFSQmdOVkJBb01Da0Z3Y0d4bElFbHVZeTR4Q3pBSkJnTlZCQVlUQWxWVE1JSUJJakFOQmdrcWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBcGMrQi9TV2lnVnZXaCswajJqTWNqdUlqd0tYRUpzczl4cC9zU2cxVmh2K2tBdGVYeWpsVWJYMS9zbFFZbmNRc1VuR09aSHVDem9tNlNkWUk1YlNJY2M4L1cwWXV4c1FkdUFPcFdLSUVQaUY0MWR1MzBJNFNqWU5NV3lwb041UEM4cjBleE5LaERFcFlVcXNTNCszZEg1Z1ZrRFV0d3N3U3lvMUlnZmRZZUZScjZJd3hOaDlLQmd4SFZQTTNrTGl5a29sOVg2U0ZTdUhBbk9DNnBMdUNsMlAwSzVQQi9UNXZ5c0gxUEttUFVockFKUXAyRHQ3K21mNy93bXYxVzE2c2MxRkpDRmFKekVPUXpJNkJBdENnbDdaY3NhRnBhWWVRRUdnbUpqbTRIUkJ6c0FwZHhYUFEzM1k3MkMzWmlCN2o3QWZQNG83UTAvb21WWUh2NGdOSkl3SURBUUFCbzRJQjF6Q0NBZE13UHdZSUt3WUJCUVVIQVFFRU16QXhNQzhHQ0NzR0FRVUZCekFCaGlOb2RIUndPaTh2YjJOemNDNWhjSEJzWlM1amIyMHZiMk56Y0RBekxYZDNaSEl3TkRBZEJnTlZIUTRFRmdRVWthU2MvTVIydDUrZ2l2Uk45WTgyWGUwckJJVXdEQVlEVlIwVEFRSC9CQUl3QURBZkJnTlZIU01FR0RBV2dCU0lKeGNKcWJZWVlJdnM2N3IyUjFuRlVsU2p0ekNDQVI0R0ExVWRJQVNDQVJVd2dnRVJNSUlCRFFZS0tvWklodmRqWkFVR0FUQ0IvakNCd3dZSUt3WUJCUVVIQWdJd2diWU1nYk5TWld4cFlXNWpaU0J2YmlCMGFHbHpJR05sY25ScFptbGpZWFJsSUdKNUlHRnVlU0J3WVhKMGVTQmhjM04xYldWeklHRmpZMlZ3ZEdGdVkyVWdiMllnZEdobElIUm9aVzRnWVhCd2JHbGpZV0pzWlNCemRHRnVaR0Z5WkNCMFpYSnRjeUJoYm1RZ1kyOXVaR2wwYVc5dWN5QnZaaUIxYzJVc0lHTmxjblJwWm1sallYUmxJSEJ2YkdsamVTQmhibVFnWTJWeWRHbG1hV05oZEdsdmJpQndjbUZqZEdsalpTQnpkR0YwWlcxbGJuUnpMakEyQmdnckJnRUZCUWNDQVJZcWFIUjBjRG92TDNkM2R5NWhjSEJzWlM1amIyMHZZMlZ5ZEdsbWFXTmhkR1ZoZFhSb2IzSnBkSGt2TUE0R0ExVWREd0VCL3dRRUF3SUhnREFRQmdvcWhraUc5Mk5rQmdzQkJBSUZBREFOQmdrcWhraUc5dzBCQVFVRkFBT0NBUUVBRGFZYjB5NDk0MXNyQjI1Q2xtelQ2SXhETUlKZjRGelJqYjY5RDcwYS9DV1MyNHlGdzRCWjMrUGkxeTRGRkt3TjI3YTQvdncxTG56THJSZHJqbjhmNUhlNXNXZVZ0Qk5lcGhtR2R2aGFJSlhuWTR3UGMvem83Y1lmcnBuNFpVaGNvT0FvT3NBUU55MjVvQVE1SDNPNXlBWDk4dDUvR2lvcWJpc0IvS0FnWE5ucmZTZW1NL2oxbU9DK1JOdXhUR2Y4YmdwUHllSUdxTktYODZlT2ExR2lXb1IxWmRFV0JHTGp3Vi8xQ0tuUGFObVNBTW5CakxQNGpRQmt1bGhnd0h5dmozWEthYmxiS3RZZGFHNllRdlZNcHpjWm04dzdISG9aUS9PamJiOUlZQVlNTnBJcjdONFl0UkhhTFNQUWp2eWdhWndYRzU2QWV6bEhSVEJoTDhjVHFBPT0iOwoJInB1cmNoYXNlLWluZm8iID0gImV3b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdVdGNITjBJaUE5SUNJeU1ESXlMVEF4TFRJMUlEQXdPak14T2pJMklFRnRaWEpwWTJFdlRHOXpYMEZ1WjJWc1pYTWlPd29KSW5WdWFYRjFaUzFwWkdWdWRHbG1hV1Z5SWlBOUlDSXdNREF3T0RBeU1DMHdNREV6TmprMU5ESXhNMEV3TURKRklqc0tDU0p2Y21sbmFXNWhiQzEwY21GdWMyRmpkR2x2YmkxcFpDSWdQU0FpTVRBd01EQXdNRGsxTlRrd056UTJNQ0k3Q2draVluWnljeUlnUFNBaU1TNHhJanNLQ1NKMGNtRnVjMkZqZEdsdmJpMXBaQ0lnUFNBaU1UQXdNREF3TURrMU5Ua3dOelEyTUNJN0Nna2ljWFZoYm5ScGRIa2lJRDBnSWpFaU93b0pJbWx1TFdGd2NDMXZkMjVsY25Ob2FYQXRkSGx3WlNJZ1BTQWlVRlZTUTBoQlUwVkVJanNLQ1NKdmNtbG5hVzVoYkMxd2RYSmphR0Z6WlMxa1lYUmxMVzF6SWlBOUlDSXhOalF6TURrNU5EZzJOakUzSWpzS0NTSjFibWx4ZFdVdGRtVnVaRzl5TFdsa1pXNTBhV1pwWlhJaUlEMGdJak5EUXpjek1UTTJMVUZEUTBFdE5FUkRNaTA0TTBGRkxUVkNPREJFUkRJME5UVXpOU0k3Q2draWNISnZaSFZqZEMxcFpDSWdQU0FpYVc4dWRtVnlaR0Z1ZEhOd1lYSnJjeTUwWlhOME1pNWhhWEl4TUNJN0Nna2lhWFJsYlMxcFpDSWdQU0FpTVRZd05qRTFPRGswTkNJN0Nna2lZbWxrSWlBOUlDSnBieTUyWlhKa1lXNTBjM0JoY210ekxuUmxjM1F5SWpzS0NTSnBjeTFwYmkxcGJuUnlieTF2Wm1abGNpMXdaWEpwYjJRaUlEMGdJbVpoYkhObElqc0tDU0p3ZFhKamFHRnpaUzFrWVhSbExXMXpJaUE5SUNJeE5qUXpNRGs1TkRnMk5qRTNJanNLQ1NKd2RYSmphR0Z6WlMxa1lYUmxJaUE5SUNJeU1ESXlMVEF4TFRJMUlEQTRPak14T2pJMklFVjBZeTlIVFZRaU93b0pJbWx6TFhSeWFXRnNMWEJsY21sdlpDSWdQU0FpWm1Gc2MyVWlPd29KSW5CMWNtTm9ZWE5sTFdSaGRHVXRjSE4wSWlBOUlDSXlNREl5TFRBeExUSTFJREF3T2pNeE9qSTJJRUZ0WlhKcFkyRXZURzl6WDBGdVoyVnNaWE1pT3dvSkltOXlhV2RwYm1Gc0xYQjFjbU5vWVhObExXUmhkR1VpSUQwZ0lqSXdNakl0TURFdE1qVWdNRGc2TXpFNk1qWWdSWFJqTDBkTlZDSTdDbjA9IjsKCSJlbnZpcm9ubWVudCIgPSAiU2FuZGJveCI7CgkicG9kIiA9ICIxMDAiOwoJInNpZ25pbmctc3RhdHVzIiA9ICIwIjsKfQ==";

            var result = await VerifyReceiptOnApple(receipt, string.Empty);

            if (result) return Ok();

            return BadRequest();
        }

        private const string AppSharedSecret = "5a96a9acee604d7d904d1d52b2b0c09a";
        private const string AppleVerifyReceiptEndpoint = "https://sandbox.itunes.apple.com/verifyReceipt";

        private async Task<bool> VerifyReceiptOnApple(string base64Str, string id) {
            string jsonStr;

            if (id.Equals("io.verdantsparks.test2.autorenew")) {
                var objToSend = new SubscriptionRequest() {
                    ReceiptData = base64Str,
                    Password = AppSharedSecret,
                };
                jsonStr = JsonConvert.SerializeObject(objToSend);
            }
            else {
                var objToSend = new Request() {
                    ReceiptData = base64Str
                };
                jsonStr = JsonConvert.SerializeObject(objToSend);
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await httpClient.PostAsync(
                new Uri(AppleVerifyReceiptEndpoint),
                new StringContent(jsonStr, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode) return false;

            var parseResult = await result.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Response>(parseResult);

            if (obj == null) return false;

            Console.WriteLine(obj.Status);
            return obj.Status == 0;

        }

        public class Request {
            [JsonProperty("receipt-data")]
            public string? ReceiptData { get; set; }
        }

        public class SubscriptionRequest : Request {
            [JsonProperty("password")]
            public string? Password { get; set; }
        }

        public class Response {
            public int Status { get; set; }

            [JsonProperty("is-retryable")]
            public int IsRetryable { get; set; }

            public string Environment { get; set; }
        }

        public class Receipt {
            /// <summary>
            /// The purchase Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// The bundle Id of the app
            /// </summary>
            public string BundleId { get; set; }

            /// <summary>
            /// The transaction Id
            /// </summary>
            public string TransactionId { get; set; }
        }

        /// <summary>
        /// A receipt for iOS in-app purchases
        /// </summary>
        public class AppleReceipt : Receipt {
            /// <summary>
            /// The binary "receipt" from Apple
            /// </summary>
            public byte[] Data { get; set; }
        }
    }
}
