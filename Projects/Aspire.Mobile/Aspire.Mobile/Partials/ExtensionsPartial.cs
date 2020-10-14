using System;

#if __IOS__
using BigTed;
#else
    using Plugin.CurrentActivity;
    using AndroidHUD;
#endif

namespace Xamarin.Forms.Core
{
    public static partial class CoreExtensions
    {
        public static void ShowLoadingDialog(this CoreViewModel model, string msg)
        {

#if __IOS__

            BTProgressHUD.Show(msg, -1, ProgressHUD.MaskType.Black);
#else
            AndHUD.Shared.Show(CrossCurrentActivity.Current.Activity, msg, (int)MaskType.Clear);
#endif
        }

        public static void CloseLoadingDialog(this CoreViewModel model)
        {

#if __IOS__
            BTProgressHUD.Dismiss();
#else
            AndHUD.Shared.Dismiss(CrossCurrentActivity.Current.Activity);
#endif
        }

        public static void ShowLoadingPercentDialog(this CoreViewModel model, string msg, float percent)
        {
#if __IOS__
            BTProgressHUD.Show(msg, percent, ProgressHUD.MaskType.Black);
#else
           
            AndHUD.Shared.Show(CrossCurrentActivity.Current.Activity, msg, (int)percent);
#endif
        }

        public static void CloseLoadingPercentDialog(this CoreViewModel model)
        {

#if __IOS__
            BTProgressHUD.Dismiss();
#else
            AndHUD.Shared.Dismiss(CrossCurrentActivity.Current.Activity);
#endif
        }
    }

}

