using System;
using System.Linq.Expressions;
using System.Windows.Input;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    //    [Export(typeof(IEntityListViewModel<>))]

    public interface IListableEntityListViewModel<T>
        where T : class, IEntity, IListableModel, new()
    {

    }

    public class ListableEntityListViewModel<T> : Core.EntityLists.EntityListViewModel<T>, IListableEntityListViewModel<T>
        where T : class, IEntity, IListableModel, new()
    {
        public ListableEntityListViewModel(Injector i) : base(i, c => c
                .HideFilters()
                .HideMenu()
                .Column("Name")
                .Header("{Name}")
                .Width(150).Localize(e => e.Caption)
                .Link(e => e.Caption)
                .Icon(e => e.IconPath)
                    .Filter()
        )
        {
        }

        public ListableEntityListViewModel(Injector i, Expression<Func<T, bool>> filter) : base(i, c => c
            .Column("Name").Header("{Name}").Content(e => e.Caption)
                .Icon(e => e.IconPath)
        )
        {
        }

    }
}