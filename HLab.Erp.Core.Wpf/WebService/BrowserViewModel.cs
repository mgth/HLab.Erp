using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using HLab.Erp.Core.WebService;
using HLab.Mvvm;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.WebService
{
    using H = H<BrowserViewModel>;

    public class BrowserViewModel : ViewModel, IBrowserService
    {
        readonly IDocumentService _docs;

        public ICommand OpenCommand { get; } = H.Command(c => c
        .Action(e => e._docs.OpenDocumentAsync(e))
        );

        public string Title => "Internet";
        public void Navigate(string url)
        {
            _docs.OpenDocumentAsync(this);
            WebBrowser.Navigate(url);
        }

        public string Url
        {
            get => _url.Get();
            set => _url.Set(value);
        }

        readonly IProperty<string> _url = H.Property<string>(c => c.Default(""));

        IProperty<FrameworkElement> _host = H.Property<FrameworkElement>(c => c
            .On(e => e.WebBrowser)
            .Set(e => (FrameworkElement)new WindowsFormsHost
            {
                Child = e.WebBrowser
            }));

        public WebBrowser WebBrowser => _webBrowser.Get();

        readonly IProperty<WebBrowser> _webBrowser = H.Property<WebBrowser>(c => c
            //.On(e => e.WebBrowser)
            .Set(e =>
            {
                var web = new WebBrowser
                {
                    ScriptErrorsSuppressed = true,
                };
                web.Navigating += e.Web_Navigating;
                web.CanGoBackChanged += (sender, args) => (e.BrowseBackCommand as ITriggerable)?.OnTriggered();
                web.CanGoForwardChanged += (sender, args) => (e.BrowseForwardCommand as ITriggerable)?.OnTriggered();

                return web;
            }));

        public BrowserViewModel(IDocumentService docs)
        {
            _docs = docs;
            H.Initialize(this);
        }

        void Web_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Url = e.Url.OriginalString;
        }

        public ICommand BrowseBackCommand { get; } = H.Command(c => c
            .CanExecute(e => e.WebBrowser.CanGoBack)
            .Action(e =>  e.WebBrowser.GoBack())
            .OnEvent((e,evt) => e.WebBrowser.CanGoBackChanged += evt)
        );

        public ICommand BrowseForwardCommand { get; } = H.Command(c => c
            .CanExecute(e => e.WebBrowser.CanGoForward)
            .Action(e => e.WebBrowser.GoForward())
            .OnEvent((e, evt) => e.WebBrowser.CanGoForwardChanged += evt)
        );

        public ICommand NavigateCommand { get; } = H.Command(c => c
            .CanExecute(e => !string.IsNullOrEmpty(e.Url))
            .Action(e => e.WebBrowser.Navigate(e.Url))
            .On(e => e.Url).CheckCanExecute()
        );
    }
}
