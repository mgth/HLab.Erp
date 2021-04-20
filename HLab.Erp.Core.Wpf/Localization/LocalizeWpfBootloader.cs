using System.Windows.Input;
using Grace.DependencyInjection.Attributes;
using HLab.Core.Annotations;
using HLab.Notify.PropertyChanged;
namespace HLab.Erp.Core.Localization
{
    using H = H<LocalizeWpfBootloader>;

    [Export(typeof(IBootloader))]
    public class LocalizeWpfBootloader : NotifierBase, IBootloader //postboot
    {
        public LocalizeWpfBootloader(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        private readonly IErpServices _erp;


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