using System;
using DeclarativeSharp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace DeclarativeSharp {
    public partial class App : Application {
        public App() {
            MainPage = new LandingPage();
        }

        protected override async void OnStart() {
            // Handle when your app starts
//            await new CafeRepo().Init();
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
