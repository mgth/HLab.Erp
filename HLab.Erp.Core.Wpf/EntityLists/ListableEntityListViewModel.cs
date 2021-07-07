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

    public class ListableEntityListViewModel<T> : EntityListViewModel<T>, IListableEntityListViewModel<T>
        where T : class, IEntity, IListableModel, new()
    {
        public ListableEntityListViewModel() : base(c => c
                .Column()
                .Header("{Name}")
                .Width(150).Content(e => e.Caption)
                .Link(e => e.Caption)
                .Icon(e => e.IconPath)
                    .Filter()
        )
        {
        }

        public ListableEntityListViewModel(Expression<Func<T, bool>> filter) : base(c => c
            .Column().Header("{Name}").Content(e => e.Caption)
                .Icon(e => e.IconPath)
        )
        {
        }

        public override ICommand AddCommand { get; } = H<ListableEntityListViewModel<T>>.Command(c => c
            .Action(async e => await e.AddEntityAsync())
        );
    }
}