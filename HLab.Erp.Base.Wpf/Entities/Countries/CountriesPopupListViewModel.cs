using System;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesPopupListViewModel : EntityListViewModel<Country>
    {
        public CountriesPopupListViewModel() : base(c => c
           .Column()
               .Header("{Name}").Width(150)
               .Content(s => s.Name)
               .Localize()
               .OrderByOrder(0)
                   .Filter<TextFilter>()
           .Column()
               .Header("{Continent}")
               .Content(c => c.Continent.Name)
                   .Filter<EntityFilterNullable<Continent>>()
                    .Link(c => c.ContinentId)
           .Column()
               .Header("{Flag}").Width(60)
               .Icon(s => s.IconPath)

        )
        {
        }
    }


}