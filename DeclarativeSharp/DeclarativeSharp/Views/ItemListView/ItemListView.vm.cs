using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DeclarativeSharp.Model;
using Xamarin.Forms;

namespace DeclarativeSharp.Views.ItemListView {
    public partial class ItemListView : ContentPage {

        public class ItemListViewModel {
            public ObservableCollection<Cafe> Cafes { get; private set; }
            public Cafe SelectCafe { get; set; }

            public async Task LoadData() {
                var repo = new CafeRepo();
                Cafes = new ObservableCollection<Cafe>(await repo.GetAll());
            }
        }
    }
}
