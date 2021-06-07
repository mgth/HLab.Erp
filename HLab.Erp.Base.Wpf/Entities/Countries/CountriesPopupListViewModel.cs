using System;

using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesPopupListViewModel : EntityListViewModel<Country>
    {
        public CountriesPopupListViewModel() : base(c => c
            .Column()
            .Header("{Name}").Width(150)
            .Link(s => s.Name)
            .Localize()
            // TODO .OrderByOrder(0)
            .Filter()

            .Column()
            .Header("{Continent}")
            .Link(e => e.Continent)
                   .Filter()

           .Column()
               .Header("{Flag}").Width(60)
               .Icon(s => s.IconPath)

        )
        {
        }
    }


}