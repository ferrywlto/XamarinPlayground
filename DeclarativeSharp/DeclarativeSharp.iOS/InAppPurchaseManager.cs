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
    public class PaymentTransaction {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public DateTime UtcDate { get; set; }

        public string State { get; set; }

        public string PurchaseToken { get; set; }
    }

    public class InAppPurchaseManager : IInAppPurchaseService {
        public async Task<ProductForPurchase[]> GetPrices(params string[] ids) {
            using var del = new RequestDelegateImpl();
            using var request = new SKProductsRequest(new NSSet(ids));
            request.Delegate = del;
            request.Start();

            return await del.Source.Task;
        }
        public async Task<Receipt> BuyNative(ProductForPurchase productForPurchase) {
            if (!SKPaymentQueue.CanMakePayments) return null;

            var source = _transactionObserver.GetTaskCompletionSource(productForPurchase.Id);
            var product = (SKProduct)productForPurchase.NativeObject;
            var payment = SKPayment.CreateFrom(product);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);

            /* It has to return more than one thing. That's why I feel so weird for just return source.Task,
             if there is exception set due to transaction error, you won't able to catch it.
             */
            return await source.Task;
        }

        /*
         * if use `using var iap = DependencyService.Get<IInAppPurchaseService>();` to obtain a reference of an instance of this class,
         * then this pair can safely release resource to prevent memory leak.
         */
        private readonly MyPaymentTransactionObserver _transactionObserver;
        public InAppPurchaseManager() {
            _transactionObserver = new MyPaymentTransactionObserver();
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(_transactionObserver);
        }
        public void Dispose() {
            SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(_transactionObserver);
            _transactionObserver?.Dispose();
        }
//        public async Task<PaymentTransaction> BuyNative(string productId)
//        {
//            var paymentTrans = await this.Purchase(productId);
//
//            var reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
//
//            var purchase = new PaymentTransaction();
//            purchase.UtcDate = reference.AddSeconds(paymentTrans.TransactionDate.SecondsSinceReferenceDate);
//            purchase.Id = paymentTrans.TransactionIdentifier;
//            purchase.ProductId = paymentTrans.Payment?.ProductIdentifier ?? string.Empty;
//            purchase.State = paymentTrans.TransactionState.ToString();
//            purchase.PurchaseToken =
//                paymentTrans.TransactionReceipt?.GetBase64EncodedString(NSDataBase64EncodingOptions.None)
//                ?? string.Empty;
//
//            return purchase;
//        }

        private Task<SKPaymentTransaction> Purchase(string productId) {
            var transaction = new TaskCompletionSource<SKPaymentTransaction>();

            //dirty code
            void Handler(SKPaymentTransaction trans, bool result) {
                if (trans?.Payment == null) return;
                if (trans.Payment.ProductIdentifier != productId) return;

                _transactionObserver.TransactionCompleted -= Handler;

                if (!result) return;

                transaction.TrySetResult(trans);

                var errorCode = trans.Error?.Code ?? -1;
                var description = trans.Error?.LocalizedDescription ?? string.Empty;

                transaction.TrySetException(
                    new Exception($"Transaction failed (errorCode: {errorCode} \r\n{description}"));
            }

            _transactionObserver.TransactionCompleted += Handler;

            SKPaymentQueue.DefaultQueue.AddPayment(SKPayment.CreateFrom(productId));

            return transaction.Task;
        }


    }

public class RequestDelegateImpl : SKProductsRequestDelegate {

        private readonly NSNumberFormatter _formatter = new NSNumberFormatter {
            FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
            NumberStyle = NSNumberFormatterStyle.Currency,
            Locale = new NSLocale("zh-TW")
        };

        public readonly TaskCompletionSource<ProductForPurchase[]> Source = new();

        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response) {
            if (response.Products == null || response.Products.Length == 0) {
                Source.TrySetException(new Exception("No products were found!"));

                return;
            }

            var list = new List<ProductForPurchase>();

            foreach (var product in response.Products) {
                var formatter = new NSNumberFormatter {
                    FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
                    NumberStyle = NSNumberFormatterStyle.Currency,
                    Locale = product.PriceLocale
                };
                Console.WriteLine(product.PriceLocale);
                list.Add(
                    new ProductForPurchase {
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
