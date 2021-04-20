using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Base.Wpf.Entities.Countries;
using HLab.Erp.Base.Wpf.Entities.Icons;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf
{
    using H = H<ErpBaseModule>;

    public class ErpBaseModule : NotifierBase, IBootloader
    {
        private readonly IErpServices _erp;

        public ErpBaseModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand CountryCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocumentAsync(typeof(ListCountryViewModel))
            ));
        public ICommand IconCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocumentAsync(typeof(IconsListViewModel))
            ));
        public ICommand CountryToolsCommand { get; } = H.Command(c => c.Action(
                e => e._erp.Docs.OpenDocumentAsync(typeof(CountryToolsViewModel))
            ));
        public void Load(IBootContext b)
        {
                _erp.Menu.RegisterMenu("param/country", "{Country}",
                    CountryCommand,
                    "Icons/Entities/Country");

                _erp.Menu.RegisterMenu("param/icons", "{Icons}",
                    IconCommand,
                    "Icons/Entities/Icon");

                _erp.Menu.RegisterMenu("tools/countrytools", "{Country tools}",
                    CountryToolsCommand,
                    "Icons/Entities/Country");
        }
    }
}
