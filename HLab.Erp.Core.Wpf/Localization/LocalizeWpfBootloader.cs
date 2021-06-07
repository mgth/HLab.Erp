using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Notify.PropertyChanged;
namespace HLab.Erp.Core.Localization
{
    using H = H<LocalizeWpfBootloader>;

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
            if(b.WaitService(_erp)) return;

            _erp.Menu.RegisterMenu("tools/localization", "{Localization}",
                LocalizationOpenDocumentCommand,
                "Icons/Entities/Localize");
        }
    }
}