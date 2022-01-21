#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DeclarativeSharp.Services;
using Foundation;
using StoreKit;

namespace DeclarativeSharp.iOS {
    public class MyPaymentTransactionObserver : SKPaymentTransactionObserver {
        public event Action<SKPaymentTransaction, bool> TransactionCompleted;

        public readonly Dictionary<string, TaskCompletionSource<Receipt>> TaskCompletionSources = new();

        private NSUrl LocalReceiptUrl => NSBundle.MainBundle.AppStoreReceiptUrl;

        private bool LocalReceiptExists => NSFileManager.DefaultManager.FileExists(LocalReceiptUrl.Path);

        private NSData LocalReceiptData => NSData.FromUrl(LocalReceiptUrl);

        // Mark the transaction as finished to avoid re-initiate purchase.
        private void FinishNonPurchasingTransactionFromDefaultQueue(SKPaymentTransaction transaction) {
            if (transaction.TransactionState != SKPaymentTransactionState.Purchasing) {
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
            }
        }

        private void HandleTransactionFailed(
            SKPaymentTransaction transaction,
            TaskCompletionSource<Receipt> taskCompletionSource
        ) {
            var exception = new Exception(transaction.Error?.LocalizedDescription ?? "Unknown Error");
            taskCompletionSource.TrySetException(exception);
        }

        private void HandleTransactionSucceed(
            SKPaymentTransaction transaction,
            TaskCompletionSource<Receipt> taskCompletionSource
        ) {
            if (!LocalReceiptExists) {
                taskCompletionSource.TrySetException(new Exception("No app store receipt file found!"));
            } else {
                taskCompletionSource.TrySetResult(GetReceipt(transaction));
            }
        }

        private Receipt GetReceipt(SKPaymentTransaction transaction) {
            return new AppleReceipt() {
                Id = transaction.Payment.ProductIdentifier,
                TransactionId = transaction.TransactionIdentifier,
                BundleId = NSBundle.MainBundle.BundleIdentifier,
                Data = LocalReceiptData.ToArray(),
            };
        }

        private TaskCompletionSource<Receipt>? GetTransactionTaskOrNull(SKPaymentTransaction transaction) {
            var productId = transaction.Payment.ProductIdentifier;

            return !TaskCompletionSources.ContainsKey(productId) ? null : TaskCompletionSources[productId];
        }

        public TaskCompletionSource<Receipt> GetTaskCompletionSource(string productId) {
            var tsc = new TaskCompletionSource<Receipt>();
            TaskCompletionSources[productId] = tsc;

            return tsc;
        }

        /// <summary>
        /// When all transactions complete (i.e. when this method return, each Task in TaskCompletionSource dictionary resolves
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="transactions"></param>
        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions) {
            foreach (var transaction in transactions) {

                FinishNonPurchasingTransactionFromDefaultQueue(transaction);
                // Not further processing the transaction if we cannot find a recorded purchase of specific product. It shouldn't happen.
                var tcsForThisPurchase = GetTransactionTaskOrNull(transaction);
                if(tcsForThisPurchase == null) continue;

                switch (transaction.TransactionState) {
                    case SKPaymentTransactionState.Failed:
                        HandleTransactionFailed(transaction, tcsForThisPurchase);
                        break;
                    // Restored only happens at Non-consumable and Auto-Renew subscription
                    case SKPaymentTransactionState.Restored:
                    case SKPaymentTransactionState.Purchased:
                        HandleTransactionSucceed(transaction, tcsForThisPurchase);
                        break;

                    case SKPaymentTransactionState.Purchasing: break;
                    case SKPaymentTransactionState.Deferred: break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
