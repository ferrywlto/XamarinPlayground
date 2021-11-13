using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace DeclarativeSharp {
    public partial class MainPage : ContentPage {
        public MainPage() {
            Content = new StackLayout() {
                Children = {
                    new Map() {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(
                            new CameraPosition(
                                new Position(22.410772, 113.980277),
                                13,
                                30,
                                60)),
                    },
                    new Label() {
                        Text = "Hello Declarative C# UI!",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                    }
                }
            };
        }
    }
}
