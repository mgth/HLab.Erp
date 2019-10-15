using System.Windows.Forms;

namespace HLab.Erp.Core.WebService
{
    public interface IBrowserService
    {
        void Navigate(string url);
        WebBrowser WebBrowser { get; }
    }
}