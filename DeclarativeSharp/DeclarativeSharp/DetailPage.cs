using System;
using DeclarativeSharp.Services;
using Xamarin.Forms;

namespace DeclarativeSharp {
    public class DetailPage : ContentPage{
        private Button GetButton() {
            var btn = new Button() {
                Text = "Buy!"
            };

            btn.Clicked += (sender, args) => throw new NotImplementedException();
            return btn;
        }
        public DetailPage(Purchase purchase) {
            Content = new StackLayout() {
                Margin = new Thickness(20),
                Children = {
                    new Label() { Text = $"Name: {purchase.Name}" },
                    new Label() { Text = $"Description: {purchase.Description}" },
                    new Label() { Text = $"Price: {purchase.Price}" },
                    GetButton(),
                }
            };
        }
    }
}
