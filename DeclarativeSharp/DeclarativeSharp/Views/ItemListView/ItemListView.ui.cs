using Xamarin.Forms;

namespace DeclarativeSharp.Views.ItemListView {
    public partial class ItemListView : ContentPage {

        private ListView cafeListView() => new ListView() {
            ItemsSource = _vm.Cafes,
        };

        private void BuildUI() =>
            Content =
                new StackLayout() { Children = {
                } };

    }
}
