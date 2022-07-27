using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Core.Wpf.EntitySelectors;
using HLab.Mvvm.Application;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountryForeignViewModel : ForeignViewModel<Country>
    {
        public CountryForeignViewModel(Injector i) : base(i)
        {
        }
    }
}
