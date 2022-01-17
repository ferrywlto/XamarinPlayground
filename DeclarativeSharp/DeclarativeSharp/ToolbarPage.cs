using Xamarin.Forms;

namespace DeclarativeSharp {
    public class ToolbarPage : ContentPage {
        public ToolbarPage() {


            Padding = 10;
            Title = "Code ToolbarItem Demo";

            var nextBtn = new ToolbarItem() {
                Text = "Next",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            };

            nextBtn.Clicked += (sender, args) => {
                Navigation.PushAsync(new MainPage());
            };

            ToolbarItems.Add(nextBtn);

            var backBtn = new Button() {
                Text = "Logout",
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            backBtn.Clicked += (sender, args) => {
                Application.Current.MainPage = new LandingPage();
            };

            Content = new StackLayout() {
                Children = { backBtn }
            };
        }
    }
}
