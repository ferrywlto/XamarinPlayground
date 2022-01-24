using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeclarativeSharp.Services;
using Foundation;
using StoreKit;

namespace DeclarativeSharp.iOS {
    public class ProductRequestDelegateImpl : SKProductsRequestDelegate {

        public readonly TaskCompletionSource<ProductForPurchase[]> TaskCompletionSource = new();

        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response) {
            if (response.Products.Length == 0) {
                TaskCompletionSource.TrySetException(new Exception("No products were found!"));

                return;
            }

            var list = new List<ProductForPurchase>();

            foreach (var product in response.Products) {
                _formatter.Locale = product.PriceLocale;

                list.Add(
                    new ProductForPurchase {
                        Id = product.ProductIdentifier,
                        Price = _formatter.StringFromNumber(product.Price),
                        Name = product.LocalizedTitle,
                        Description = product.LocalizedDescription,
                        NativeObject = product,
                    });
            }

            TaskCompletionSource.TrySetResult(list.ToArray());
        }

        public override void RequestFailed(SKRequest request, NSError error) {
            TaskCompletionSource.TrySetException(new Exception(error?.LocalizedDescription ?? "Unknown Error"));
        }

        private readonly NSNumberFormatter _formatter = new() {
            FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
            NumberStyle = NSNumberFormatterStyle.Currency,
        };
    }
}
