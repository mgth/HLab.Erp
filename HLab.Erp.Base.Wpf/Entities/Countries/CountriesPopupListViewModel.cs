
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesPopupListViewModel : EntityListViewModel<Country>
    {
        public CountriesPopupListViewModel(ILocalizationService localization) : base(c => c
                .Header("{Country}")
                .Column("Name")
                    .Header("{Name}")
                    .Content(s => s.Name).Localize()
                    .Link(s => s.Name)
                    
                    // TODO                .OrderByOrder(0)
                    .Filter()
                    .PostLink(s => localization.Localize(s.Name))

            //.Column()
            //.Header("{Continent}")
            //.Link(e => e.Continent)
            //       .Filter()

           .Column("Flag")
               .Header("{Flag}")
               .Icon(s => s.IconPath, 50)

        )
        {
        }
    }


}