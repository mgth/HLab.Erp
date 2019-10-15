using System.Windows.Forms;

namespace HLab.Erp.Core.Wpf.WebService
{
    public interface IBrowserService
    {
        void Navigate(string url);
        WebBrowser WebBrowser { get; }
    }
}