using System;
using System.Net;
using System.Threading.Tasks;
using DeclarativeSharp.Services;
using Foundation;
using StoreKit;

namespace DeclarativeSharp.iOS {
    public class TransactionStateHandler {
        private readonly PurchaseTaskManager _purchaseTaskManager;
        public TransactionStateHandler(PurchaseTaskManager purchaseTaskManager) { _purchaseTaskManager = purchaseTaskManager; }

        public void HandleTransactionState(SKPaymentTransaction transaction) {
            var tcs = _purchaseTaskManager.GetRunningPurchaseTask(transaction.Payment.ProductIdentifier);

            switch (transaction.TransactionState) {
                case SKPaymentTransactionState.Failed:
                    HandleTransactionFailed(transaction, tcs);
                    SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                    break;

                case SKPaymentTransactionState.Purchased:
                    HandleTransactionSucceed(transaction, tcs);
                    // TODO: Verify receipt at server before finish transaction
                    // if fail then TrySetException
                    SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                    break;

                // Restored only happens at Non-consumable and Auto-Renew subscription
                case SKPaymentTransactionState.Restored:
                    // TODO: Verify receipt at server before finish transaction
                    // if fail then TrySetException
                    SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                    break;


                case SKPaymentTransactionState.Purchasing: break;
                case SKPaymentTransactionState.Deferred: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private async void HandleTransactionSucceed(
            SKPaymentTransaction transaction,
            TaskCompletionSource<Receipt> taskCompletionSource
        ) {
            if (!LocalReceiptExists) {
                taskCompletionSource.TrySetException(new Exception("No app store receipt file found!"));
            } else {
                var receipt = (AppleReceipt)GetReceipt(transaction);
                var util = new NetworkUtil();
                var result = await util.VerifyReceipt(receipt);

                if (result) { taskCompletionSource.TrySetResult(receipt); }
            }
        }
        private void HandleTransactionFailed(
            SKPaymentTransaction transaction,
            TaskCompletionSource<Receipt> taskCompletionSource
        ) {
            var exception = new Exception(transaction.Error?.LocalizedDescription ?? "Unknown Error");
            taskCompletionSource.TrySetException(exception);
        }

        private Receipt GetReceipt(SKPaymentTransaction transaction) {

            return new AppleReceipt() {
                Id = transaction.Payment.ProductIdentifier,
                TransactionId = transaction.TransactionIdentifier,
                BundleId = NSBundle.MainBundle.BundleIdentifier,
                Data = transaction.TransactionReceipt?.ToArray(),
            };
        }

        private bool LocalReceiptExists => NSFileManager.DefaultManager.FileExists(LocalReceiptUrl.Path);
        private NSData LocalReceiptData => NSData.FromUrl(LocalReceiptUrl);
        private NSUrl LocalReceiptUrl => NSBundle.MainBundle.AppStoreReceiptUrl;
    }
}
