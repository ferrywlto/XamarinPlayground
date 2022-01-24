using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeclarativeSharp.Services;

namespace DeclarativeSharp.iOS {
    public class PurchaseTaskManager {

        private readonly Dictionary<string, TaskCompletionSource<Receipt>> _taskCompletionSources = new();
        public TaskCompletionSource<Receipt> GetRunningPurchaseTask(string productId) {
            if (!_taskCompletionSources.ContainsKey(productId))
                throw new Exception($"Task for {productId} not exist.");

            var tcs = _taskCompletionSources[productId];

            return tcs;
        }

        public TaskCompletionSource<Receipt> StartNewPurchaseTask(string productId) {
            CancelNonCompletedPurchaseTask(productId);

            var tcs = new TaskCompletionSource<Receipt>();
            _taskCompletionSources.Add(productId, tcs);

            return tcs;
        }

        private void CancelNonCompletedPurchaseTask(string productId) {
            if (!_taskCompletionSources.ContainsKey(productId)) return;

            var tcs = _taskCompletionSources[productId];

            if (!tcs.Task.IsCompleted) {
                tcs.SetCanceled();
            }

            _taskCompletionSources.Remove(productId);
        }
    }
}
