using System;
using Aspire.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Core;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Aspire.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "Markup_Experimental" });

            MainPage = new NavigationPage(new FontDemo());
        }

    }
}
