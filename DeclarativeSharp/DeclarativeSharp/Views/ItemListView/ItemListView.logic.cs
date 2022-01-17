using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace DeclarativeSharp.Views.ItemListView {
    public partial class ItemListView : ContentPage {
        private readonly ItemListViewModel _vm;
        public ItemListView() {
            On<iOS>().SetUseSafeArea(true);

            BindingContext = _vm = new ItemListViewModel();
            BuildUI();
        }

        protected override async void OnAppearing() {
            await _vm.LoadData();
            base.OnAppearing();
        }
    }
}
