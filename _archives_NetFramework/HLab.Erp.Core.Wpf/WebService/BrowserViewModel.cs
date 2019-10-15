using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Icons;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.WebService
{
    [Export(typeof(IBrowserService)), Singleton]
    public class BrowserViewModel : ViewModel<BrowserViewModel>, IBrowserService
    {
        [Import]
        private readonly IIconService _icons;
        [Import]
        private readonly IDocumentService _docs;
        [Import]
        private readonly IMenuService _menus;
        [Import]
        private readonly Func<Action<object>, Func<bool>, ICommand> _getCommand;

        public ICommand OpenCommand => _openCommand.Get();
        private IProperty<ICommand> _openCommand = H.Property<ICommand>(c => c
        .Command(e => e._docs.OpenDocument(e),e => true)
        );

        public string Title => "Internet";
        public void Navigate(string url)
        {
            _docs.OpenDocument(this);
            WebBrowser.Navigate(url);
        }

        public string Url
        {
            get => _url.Get();
            set => _url.Set(value);
        }
        private IProperty<string> _url = H.Property<string>(c => c.Default(""));

        private IProperty<FrameworkElement> _host = H.Property<FrameworkElement>(c => c
            .On(e => e.WebBrowser)
            .Set(e => new WindowsFormsHost
            {
                Child = e.WebBrowser
            }));

        public WebBrowser WebBrowser => _webBrowser.Get();
        private IProperty<WebBrowser> _webBrowser = H.Property<WebBrowser>(c => c
            //.On(e => e.WebBrowser)
            .Set(e =>
            {
                var web = new WebBrowser
                {
                    ScriptErrorsSuppressed = true,
                };
                web.Navigating += e.Web_Navigating;
                web.CanGoBackChanged += (sender, args) => (e.BrowseBackCommand as ITriggable)?.OnTrigged();
                web.CanGoForwardChanged += (sender, args) => (e.BrowseForwardCommand as ITriggable)?.OnTrigged();

                return web;
            }));

        private void Web_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Url = e.Url.OriginalString;
        }

        public ICommand BrowseBackCommand => _browseBackCommand.Get();
        private IProperty<ICommand> _browseBackCommand = H.Property<ICommand>(c => c
            .OnEvent((e,evt) => e.WebBrowser.CanGoBackChanged += evt)
            .Command(
                e => e.WebBrowser.GoBack(),
                e => e.WebBrowser.CanGoBack
            )
        );
        public ICommand BrowseForwardCommand => _browseForwardCommand.Get();
        private IProperty<ICommand> _browseForwardCommand = H.Property<ICommand>(c => c
            .OnEvent((e,evt) => e.WebBrowser.CanGoForwardChanged += evt)
            .Command(
                e => e.WebBrowser.GoForward(),
                e => e.WebBrowser.CanGoForward
            )
        );

        public ICommand NavigateCommand => _navigateCommand.Get();
        private IProperty<ICommand> _navigateCommand = H.Property<ICommand>(c => c
            .On(e => e.Url)
            .Command(e => e.WebBrowser.Navigate(e.Url),e=> !string.IsNullOrEmpty(e.Url)
            )
        );
    }
}
