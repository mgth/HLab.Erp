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
        
        [Import] private readonly IErpServices _erp;


        public ICommand LocalizationOpenDocumentCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                e._erp.Docs.OpenDocumentAsync(typeof(LocalizationViewModel));
            })
        );

        public void Load(IBootContext b)
        {
            _erp.Menu.RegisterMenu("tools/localization", "{Localization}",
                LocalizationOpenDocumentCommand,
                "Icons/Entities/Localize");
        }
    }
}