using System.ComponentModel;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace DeclarativeSharp.Views.BindingTest {
    public class BindingTestPage : ContentPage {
        public class BindingTestViewModel : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private int _sliderValue;
            public int SliderValue {
                get => _sliderValue;
                set {
                    if (_sliderValue == value) return;

                    _sliderValue = value;
                    OnPropertyChanged(nameof(SliderValue));
                }
            }

            public BindingTestViewModel() {
                SliderValue = 0;
            }
        }

        private BindingTestViewModel vm;

        public BindingTestPage() {
            // IMPORTANT!
            BindingContext = vm = new BindingTestViewModel();

            Content = new StackLayout() {
                Children = {
                    new Label() {
                            Text = "Should replace me.",
                            // You can override BindingContext here!
                            // BindingContext = anotherViewModel,
                        }
                        .Bind(Label.TextProperty, nameof(vm.SliderValue)
                            // Override here takes highest precedence.
                            // source: yetAnotherViewModel
                            ),
                    MySlider,
                    new Button() { Text = "Add"}.Invoke(button => button.Clicked += (_, _) => {
                        vm.SliderValue += 1;
                    }),
                },
            };
        }

        public Slider MySlider =>
            new Slider() { Maximum = 100, }
                .Bind(Slider.ValueProperty, nameof(vm.SliderValue), BindingMode.TwoWay);
    }
}
