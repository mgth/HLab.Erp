
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesPopupListViewModel : Core.EntityLists.EntityListViewModel<Country>
    {
        public CountriesPopupListViewModel(Injector i) : base(i, c => c
            .HideMenu()
                .Header("{Country}")
                .Column("Name")
                    .Header("{Name}")
                    .Localize(s => s.Name)
                    .Link(s => s.Name)
                    
                    // TODO                .OrderByOrder(0)
                    .Filter()
                    .PostLink(s => i.Localization.Localize(s.Name))

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