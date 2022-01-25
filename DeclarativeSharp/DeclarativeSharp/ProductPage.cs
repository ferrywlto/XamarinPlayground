using System;
using System.Collections.ObjectModel;
using DeclarativeSharp.Services;
using Xamarin.Forms;

namespace DeclarativeSharp {
    public class ProductPage : ContentPage {

        public ObservableCollection<ProductForPurchase> AvailablePurchases = new ObservableCollection<ProductForPurchase>();
        public ListView purchaseView;

        public ProductPage() {
            purchaseView = new ListView {
                RowHeight = 60,
                ItemTemplate = new DataTemplate(
                    () => {
                        var grid = new Grid() { Margin = new Thickness(5,10)};
                        var lblName = new Label();
                        var lblDesc = new Label() { FontSize = lblName.FontSize * 0.7, TextColor = Color.Gray};
                        var btnBuy = new Button() { HorizontalOptions = LayoutOptions.End};

                        lblName.SetBinding(Label.TextProperty, "Name");
                        lblDesc.SetBinding(Label.TextProperty, "Description");
                        btnBuy.SetBinding(Button.TextProperty, "Price", stringFormat: "以 {0} 購買");


                        btnBuy.Clicked += async (sender, args) => {
                            var _btn = (Button)sender;
                            var purchase = (ProductForPurchase)_btn.BindingContext;
                            await Navigation.PushAsync(new DetailPage(purchase));
                        };

                        var innerGrid = new Grid();
                        innerGrid.Children.Add(lblName);
                        innerGrid.Children.Add(lblDesc, 0, 1);

                        grid.Children.Add(innerGrid);
                        grid.Children.Add(btnBuy, 1, 0);

                        return new ViewCell() { View = grid };
                    })
            };

            purchaseView.ItemsSource = AvailablePurchases;
            var btn = new Button() {
                Text = "Show Products",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Start
            };
            btn.Clicked += BtnOnClicked;

//            var btnNetwork = new Button() {
//                Text = "Connect",
//                HorizontalOptions = LayoutOptions.StartAndExpand,
//                VerticalOptions = LayoutOptions.Start
//            };
//
//            btnNetwork.Clicked += async (sender, args) => {
//                var network = new NetworkUtil();
//                await network.VerifyReceipt(new AppleReceipt() {
//                    Id = "1", BundleId = "io.verdantsparks.test2", TransactionId = "2130918", Data = new byte[1]
//                });
//            };

            Title = "Product Page";

            Content = new StackLayout() {
                Margin = new Thickness(15,30),
                Children = {
                    btn, purchaseView
                }
            };
        }

        private async void BtnOnClicked(object sender, EventArgs e) {
            AvailablePurchases.Clear();
            var iap = DependencyService.Get<IInAppPurchaseService>();
            const string productId1 = "io.verdantsparks.test2.air1";
            const string productId2 = "io.verdantsparks.test2.air10";
            // These two will not retrieve
            const string productId3 = "io.verdantsparks.test2.level";
            const string productId4 = "io.verdantsparks.test2.autorenew";
            var prices = await iap.GetPrices(productId1, productId2, productId3, productId4);

            foreach (var price in prices) {
                AvailablePurchases.Add(price);
            }
        }

    }
}
