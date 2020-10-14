using Xamarin.Forms;
using Xamarin.Forms.Markup;

namespace Aspire.Mobile
{

    public class Convert
    {
        public static FuncConverter<string, string> UpperCase = new FuncConverter<string, string>(v =>
        {
            if (!string.IsNullOrEmpty(v))
                return v.ToUpper();
            else
                return v;
        });

        public static FuncConverter<bool, SeparatorVisibility> BoolToSeparatorVisibility = new FuncConverter<bool, SeparatorVisibility>(v =>
        {
            return v == true ? SeparatorVisibility.Default : SeparatorVisibility.None;
        });
    }
}

