using StoreKit;

namespace DeclarativeSharp.iOS {
    public class MyPaymentTransactionObserver : SKPaymentTransactionObserver {
        private readonly TransactionStateHandler _transactionStateHandler;
        public MyPaymentTransactionObserver(TransactionStateHandler transactionStateHandler) { _transactionStateHandler = transactionStateHandler; }

        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions) {
            foreach (var transaction in transactions) {
                FinishCompletedTransactionFromDefaultQueue(transaction);
                _transactionStateHandler.HandleTransactionState(transaction);
            }
        }

        // Mark the transaction as finished to avoid re-initiate purchase.
        private static void FinishCompletedTransactionFromDefaultQueue(SKPaymentTransaction transaction) {
            if (transaction.TransactionState != SKPaymentTransactionState.Purchasing &&
                transaction.TransactionState != SKPaymentTransactionState.Deferred) {
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
            }
        }
    }
}
