using System;
using DeclarativeSharp.Views.BindingTest;
using Xamarin.Forms;

namespace DeclarativeSharp {
    public class LandingPage : ContentPage{
        public LandingPage() {
            var btn = new Button() {
                Text = "Click Me",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            btn.Clicked += BtnOnClicked;

            Title = "Landing Page";

            Content = new StackLayout() {
                Children = {
                    btn
                }
            };
        }

        private void BtnOnClicked(object sender, EventArgs e) {
            Application.Current.MainPage = new NavigationPage(new BindingTestPage());
        }
    }
}
