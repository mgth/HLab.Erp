using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Core.Localization.LocalizeWpfBootloader>;
namespace HLab.Erp.Core.Localization
{
    public class LocalizeWpfBootloader : IBootloader //postboot
    {
        
        private readonly IErpServices _erp;

        [Import] public LocalizeWpfBootloader(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand LocalizationOpenDocumentCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                e._erp.Docs.OpenDocumentAsync(typeof(LocalizationViewModel));
            })
        );

        public void Load()
        {
            _erp.Menu.RegisterMenu("tools", "localization", "{Localization}",
                LocalizationOpenDocumentCommand,
                "icons/localize");
        }
    }
}