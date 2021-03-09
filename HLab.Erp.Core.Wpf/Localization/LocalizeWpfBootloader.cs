using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;
namespace HLab.Erp.Core.Localization
{
    using H = H<LocalizeWpfBootloader>;
    public class LocalizeWpfBootloader : NotifierBase, IBootloader //postboot
    {
        public LocalizeWpfBootloader() => H.Initialize(this);

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