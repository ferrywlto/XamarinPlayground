using System;
using StoreKit;

namespace DeclarativeSharp.iOS {
    public class MyPaymentTransactionObserverDeprecated: SKPaymentTransactionObserver {
        public event Action<SKPaymentTransaction, bool> TransactionCompleted;

        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions) {
            var canMakePayments = SKPaymentQueue.CanMakePayments;

            foreach (var transaction in transactions) {
                if (transaction?.TransactionState == null) break;

                switch (transaction.TransactionState) {
                    case SKPaymentTransactionState.Restored:
                    case SKPaymentTransactionState.Purchasing:
                    case SKPaymentTransactionState.Purchased:
                        TransactionCompleted?.Invoke(transaction, true);
                        SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                        break;

                    case SKPaymentTransactionState.Deferred:
                    case SKPaymentTransactionState.Failed:
                        TransactionCompleted?.Invoke(transaction, false);
                        SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
