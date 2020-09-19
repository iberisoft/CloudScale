using System.Threading.Tasks;
using Xamarin.Forms;

namespace CloudScale
{
    static class PageExtention
    {
        public static Task Inform(this Page page, string title, string message) => page.DisplayAlert(title, message, "OK");

        public static Task<bool> Confirm(this Page page, string title, string message) => page.DisplayAlert(title, message, "Yes", "No");
    }
}
