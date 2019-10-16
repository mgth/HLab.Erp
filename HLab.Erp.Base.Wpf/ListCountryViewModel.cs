using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.ViewModels;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Base.Wpf
{
    class ListCountryViewModel : EntityListViewModel<ListCountryViewModel,Country>, IMvvmContextProvider
    {
        [Import]
        private IIconService _icons;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public string Title => "^Country";

        public ListCountryViewModel()
        {
            Columns
                .Column("^Name", s => new Localize{Id=s.Name})
                .Column("^A2 Code", s => s.IsoA2)
                .Column("^A3 Code ", s => s.IsoA3)
                .Column("^Code", s => s.Iso)
                .Column("^Continent", s => new Localize{Id = s.Continent.Name})
                .Column("^Flag", s => new IconView{
                    MaxWidth = 50,
                    MinHeight = 50,
                    Id = s.Icon
                });

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.Update();
        }
    }
}