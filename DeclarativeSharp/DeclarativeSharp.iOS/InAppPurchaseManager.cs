using System.Threading.Tasks;
using DeclarativeSharp.Services;
using Foundation;
using StoreKit;

// The register by method approach is hard to notice.
// Better use registration by method approach
// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/registration-and-resolution
//[assembly: Dependency(typeof(InAppPurchaseManager))]

namespace DeclarativeSharp.iOS {
    // This is because it will instantiated by DI container in Shared project.
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InAppPurchaseManager : IInAppPurchaseService {
        private readonly MyPaymentTransactionObserver _transactionObserver;
        private readonly PurchaseTaskManager _purchaseTaskManager;
        public InAppPurchaseManager() {
            _purchaseTaskManager = new PurchaseTaskManager();
            _transactionObserver = new MyPaymentTransactionObserver(new TransactionStateHandler(_purchaseTaskManager));
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(_transactionObserver);
        }

        public bool CanMakePayments() => SKPaymentQueue.CanMakePayments;

        public async Task<ProductForPurchase[]> GetPrices(params string[] ids) {
            using var del = new ProductRequestDelegateImpl();
            using var request = new SKProductsRequest(new NSSet(ids));
            request.Delegate = del;
            request.Start();

            return await del.TaskCompletionSource.Task;
        }

        public async Task<Receipt> BuyNativeMultiple(ProductForPurchase productForPurchase, int quantity = 1) {
            if (!SKPaymentQueue.CanMakePayments) return null;

            var tcs = _purchaseTaskManager.StartNewPurchaseTask(productForPurchase.Id);
            var payment = SKMutablePayment.PaymentWithProduct(productForPurchase.Id);
            payment.Quantity = quantity;
            SKPaymentQueue.DefaultQueue.AddPayment(payment);

            return await tcs.Task;
        }

        public async Task<Receipt> BuyNative(ProductForPurchase productForPurchase) {
            return await BuyNativeMultiple(productForPurchase);
        }

        public void Dispose() {
            SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(_transactionObserver);
            _transactionObserver?.Dispose();
        }
    }
}
