using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Notify.PropertyChanged;
namespace HLab.Erp.Core.WebService
{
    using H = H<BrowserServiceModule>;

    public class BrowserServiceModule : NotifierBase, IBootloader
    {
        private readonly IErpServices _erp;

        public BrowserServiceModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand OpenDocumentCommand { get; } = H.Command(c => c
            .Action(e => e._erp.Docs.OpenDocumentAsync(typeof(IBrowserService)))
        );

        public void Load(IBootContext b) => _erp.Menu.RegisterMenu("tools/internet", "{Internet}", OpenDocumentCommand, "icons/internet");
        
    }
}
