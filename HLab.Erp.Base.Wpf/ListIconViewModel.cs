using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Wpf.ViewModels;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf.Icons;

namespace HLab.Erp.Base.Wpf
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
                .Column("^Icon", s => _icons.FromSvgString(s.Source))
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