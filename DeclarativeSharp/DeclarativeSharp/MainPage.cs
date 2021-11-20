using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeclarativeSharp.Model;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace DeclarativeSharp {
    public partial class MainPage : ContentPage {

        public async Task<List<Label>> BuildCafeList() {
            var repo = new CafeRepo();
            var list = await repo.GetAll();
            var labels = list.Select(cafe => new Label() {Text = cafe.Name}).ToList();

            return labels;
        }

        protected override async void OnAppearing() {
            var layout = new StackLayout() {
                Children = {
                    new Label() {
                        Text = "Hello Declarative C# UI!",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                    },
                    new Map() {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(
                            new CameraPosition(
                                new Position(22.410772, 113.980277),
                                13,
                                30,
                                60)),
                    },
                }
            };
            var labels = await BuildCafeList();
            labels.ForEach(l => layout.Children.Add(l));

        }

        public MainPage() {
            Title = "Code ToolbarItem Demo";
            ToolbarItems.Add(new ToolbarItem() {
                Text = "Add",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
            });
        }
    }
}
