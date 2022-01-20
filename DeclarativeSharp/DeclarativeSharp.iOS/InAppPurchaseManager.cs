using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeclarativeSharp.iOS;
using DeclarativeSharp.Services;
using Foundation;
using StoreKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(InAppPurchaseManager))]

namespace DeclarativeSharp.iOS {
    public class InAppPurchaseManager : IInAppPurchaseService {
        public async Task<Purchase[]> GetPrices(params string[] ids) {
            using var del = new RequestDelegateImpl();
            using var request = new SKProductsRequest(new NSSet(ids));
            request.Delegate = del;
            request.Start();

            return await del.Source.Task;
        }

        public Task<Receipt> BuyNative(Purchase purchase) => throw new NotImplementedException();
    }

    public class RequestDelegateImpl : SKProductsRequestDelegate {

        private readonly NSNumberFormatter _formatter = new NSNumberFormatter {
            FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
            NumberStyle = NSNumberFormatterStyle.Currency,
            Locale = new NSLocale("zh-TW")
        };

        public readonly TaskCompletionSource<Purchase[]> Source = new TaskCompletionSource<Purchase[]>();

        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response) {
            if (response.Products == null || response.Products.Length == 0) {
                Source.TrySetException(new Exception("No products were found!"));

                return;
            }

            var list = new List<Purchase>();

            foreach (var product in response.Products) {
                var formatter = new NSNumberFormatter {
                    FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
                    NumberStyle = NSNumberFormatterStyle.Currency,
                    Locale = product.PriceLocale
                };
                Console.WriteLine(product.PriceLocale);
                list.Add(
                    new Purchase {
                        Id = product.ProductIdentifier,
                        Price = formatter.StringFromNumber(product.Price),
                        Name = product.LocalizedTitle,
                        Description = product.LocalizedDescription,
                        NativeObject = product,
                    });
            }

            Source.TrySetResult(list.ToArray());
        }

        public override void RequestFailed(SKRequest request, NSError error) {
            Source.TrySetException(new Exception(error?.LocalizedDescription ?? "Unknown Error"));
        }
    }
}
