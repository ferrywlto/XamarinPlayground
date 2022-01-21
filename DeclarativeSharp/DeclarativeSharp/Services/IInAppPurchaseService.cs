using System;
using System.Threading.Tasks;

namespace DeclarativeSharp.Services {
    public interface IInAppPurchaseService : IDisposable {
        Task<ProductForPurchase[]> GetPrices(params string[] ids);
        Task<Receipt> BuyNative(ProductForPurchase productForPurchase);
    }


    public class ProductForPurchase {
        public string Id { get; set; }

        /// <summary>
        /// The localized price to display on the UI
        /// </summary>
        public string Price { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// In the case of iOS, an SKPayment object will be placed here
        /// </summary>
        public object NativeObject { get; set; }
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

    /// <summary>
    /// A receipt for Google Play in-app purchases
    /// </summary>
    public class GoogleReceipt : Receipt {
        /// <summary>
        /// The "developer payload" used on the purchase
        /// </summary>
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// The receipt data for Google Play
        /// </summary>
        public string PurchaseToken { get; set; }
    }
}
