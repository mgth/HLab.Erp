using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core.WebService;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.WebService
{
    using H = H<BrowserServiceModule>;

    public class BrowserServiceModule : NotifierBase, IBootloader
    {
        readonly IDocumentService _docs;
        readonly IMenuService _menu;

        public BrowserServiceModule(IDocumentService docs, IMenuService menu)
        {
            _docs = docs;
            _menu = menu;
            H.Initialize(this);
        }

        public ICommand OpenDocumentCommand { get; } = H.Command(c => c
            .Action(e => e._docs.OpenDocumentAsync(typeof(IBrowserService)))
        );

        public void Load(IBootContext b) => _menu.RegisterMenu("tools/internet", "{Internet}", OpenDocumentCommand, "icons/internet");
        
    }
}
