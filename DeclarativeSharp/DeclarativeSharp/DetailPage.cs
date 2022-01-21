using System;
using DeclarativeSharp.Services;
using Xamarin.Forms;

namespace DeclarativeSharp {
    public class DetailPage : ContentPage {
        private ProductForPurchase _productForPurchase;
        private Button GetButton() {
            var btn = new Button() {
                Text = "Buy!"
            };

            btn.Clicked += async (sender, args) => {
                using var iap = DependencyService.Get<IInAppPurchaseService>();
                var result = await iap.BuyNative(_productForPurchase);

                Console.WriteLine(result);
            };
            return btn;
        }
        public DetailPage(ProductForPurchase productForPurchase) {
            _productForPurchase = productForPurchase;
            Content = new StackLayout() {
                Margin = new Thickness(20),
                Children = {
                    new Label() { Text = $"Name: {productForPurchase.Name}" },
                    new Label() { Text = $"Description: {productForPurchase.Description}" },
                    new Label() { Text = $"Price: {productForPurchase.Price}" },
                    GetButton(),
                }
            };
        }
    }
}
