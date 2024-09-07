using System;
using System.Linq.Expressions;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Mvvm.Application;

namespace HLab.Erp.Core.EntityLists;

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