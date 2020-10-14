using System;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Helpers;


namespace Aspire.Mobile
{

    public class LandingPage: ContentPage
    {
        public LandingPage()
        {
            
            NavigationPage.SetHasNavigationBar(this, false);
            Content = new Grid()
            {
                RowDefinitions =
                {
                    Define.Row("40*"),
                    Define.Row("60*")
                },
                Children =
                {
                    new CachedImage
                    {
                        Aspect = Aspect.Fill,
                        Source = ImageSource.FromFile("lobbyimage.png")
                    }.Row(0),
                    new Grid()
                    {
                        RowDefinitions =
                        {
                            Define.Row(35),
                            Define.Row(35),
                            Define.Row(GridLength.Auto)
                        },
                        ColumnDefinitions =
                        {
                            Define.Column(GridLength.Star),
                            Define.Column(GridLength.Star)
                        },
                        Children =
                        {
                            new StackLayout()
                            {

                            }.Row(1).Column(1),
                            new Label()
                            {
                                Text = "Aspire",
                                FontSize = 33,
                                TextColor = Color.Black,
                                FontFamily = "MyAwesomeCustomFont"
                            }.RowSpan(3).ColumnSpan(2)
                        }
                        
                    }

                }
            };
        }
    }
}
