using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountryViewModel : EntityViewModel<CountryViewModel,Country>
    {
        public string Title => _title.Get();

        private readonly IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Name)
            .Set(e => "{Country} - " + e.Model.Name)
        );
    }
}
