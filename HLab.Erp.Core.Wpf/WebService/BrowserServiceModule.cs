using System;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Core.WebService.BrowserServiceModule>;
namespace HLab.Erp.Core.WebService
{
    public class BrowserServiceModule : IBootloader //postboot
    {
        private readonly IErpServices _erp;

        [Import]public BrowserServiceModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand OpenDocumentCommand = H.Command(c => c
            .Action(e => e._erp.Docs.OpenDocumentAsync(typeof(IBrowserService)))
        );

        public void Load()
        {
            _erp.Menu.RegisterMenu("tools/internet", "{Internet}", OpenDocumentCommand, "icons/internet");
        }
    }
}
