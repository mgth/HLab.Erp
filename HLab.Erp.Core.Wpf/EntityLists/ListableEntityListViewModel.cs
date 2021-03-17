using System;
using System.Linq.Expressions;
using System.Windows.Input;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.EntityLists
{
    public class ListableEntityListViewModel<T> : EntityListViewModel<T>
        where T : class, IEntity, IListableModel, new()
    {
        public ListableEntityListViewModel()
        {
            Columns.Configure(c => c

                .Column.Header("{Name}").Width(150)
                .Content(e => e.Caption).Icon(e => e.IconPath)
            );

            using(List.Suspender.Get())
                Filter<TextFilter>().Title("{Name}").Link(List, e => e.Caption);
        }
        public ListableEntityListViewModel(Expression<Func<T,bool>> filter)
        {
            List.AddFilter(()=>filter);

            Columns.Configure(c => c

                .Column.Header("{Name}")
                .Content(e => e.Caption)
                .Icon(e => e.IconPath)
            );
        }

        public override ICommand AddCommand {get; }  = H<ListableEntityListViewModel<T>>.Command(c => c
            .Action(async e => await e.AddEntityAsync())
        );
    }
}