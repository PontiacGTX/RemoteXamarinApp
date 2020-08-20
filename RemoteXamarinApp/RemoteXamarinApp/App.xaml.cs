using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteXamarinApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
           // DependencyService.Register<MockDataStore>();
            MainPage =  new DefaultPage();

        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
