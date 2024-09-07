using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core.WebService;
using HLab.Mvvm;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;


namespace HLab.Erp.Core.Wpf.WebService;

public class BrowserViewModel : ViewModel, IBrowserService
{
    public BrowserViewModel(IDocumentService docs)
    {
        _docs = docs;
            
        OpenCommand = ReactiveCommand.CreateFromTask<object>((doc, e) => docs.OpenDocumentAsync(doc));

        WebBrowser = GetWebBrowser();

        WebBrowser.CanGoBackChanged += (sender, args) => this.RaisePropertyChanged(nameof(CanGoBack));
        WebBrowser.CanGoForwardChanged += (sender, args) => this.RaisePropertyChanged(nameof(CanGoForward));

        BrowseBackCommand = ReactiveCommand.Create(
            () => WebBrowser.GoBack(),
            this.WhenAnyValue(e => e.CanGoBack)
            );

        BrowseForwardCommand = ReactiveCommand.Create(
            () => WebBrowser.GoForward(),
            this.WhenAnyValue(e => e.CanGoForward)
            );

        NavigateCommand = ReactiveCommand.Create(
            () => WebBrowser.Navigate(Url), 
            this.WhenAny(e => e.Url, e => !string.IsNullOrEmpty(e.Value))
            );
    }

    readonly IDocumentService _docs;

    public ICommand OpenCommand { get; } 

    public string Title => "Internet";
    public void Navigate(string url)
    {
        _docs.OpenDocumentAsync(this);
        WebBrowser.Navigate(url);
    }

    public string Url
    {
        get => _url;
        set => SetAndRaise(ref _url,value);
    }
    string _url = "";

    //IProperty<FrameworkElement> _host = H.Property<FrameworkElement>(c => c
    //    .On(e => e.WebBrowser)
    //    .Set(e => (FrameworkElement)new WindowsFormsHost
    //    {
    //        Child = e.WebBrowser
    //    }));

    public WebBrowser WebBrowser { get; }

    WebBrowser GetWebBrowser()
    {
        var web = new WebBrowser
        {
            ScriptErrorsSuppressed = true,
        };
        web.Navigating += Web_Navigating;
        web.CanGoBackChanged += (sender, args) => (BrowseBackCommand as ITriggerable)?.OnTriggered();
        web.CanGoForwardChanged += (sender, args) => (BrowseForwardCommand as ITriggerable)?.OnTriggered();
        return web;
    }

    void Web_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        Url = e.Url.OriginalString;
    }

    public ICommand BrowseBackCommand { get; }
    public ICommand BrowseForwardCommand { get; } 

    public ICommand NavigateCommand { get; }

    public bool CanGoBack => WebBrowser.CanGoBack;
    public bool CanGoForward => WebBrowser.CanGoForward;
}