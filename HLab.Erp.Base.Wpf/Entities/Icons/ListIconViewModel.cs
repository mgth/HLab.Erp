﻿using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Icons
{
    public class ListIconViewModel : EntityListViewModel<ListIconViewModel,Icon>, IMvvmContextProvider
    {
        [Import]
        public IIconService _icons;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
        public string Title => "^Icons";

        public ListIconViewModel()
        {
            Columns
                .Column("^Name", s => s.Name)
                .Column("^Country", s => s.Format)
                .Column("^Icon", async s => await _icons.FromSvgString(s.Source),null)
                ;

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.Update();
        }
    }
}