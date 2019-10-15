using System;
using HLab.Notify.PropertyChanged;
using System.Linq.Expressions;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Data
{
    public static class PropertyEntityExtension
    {
        public static NotifyConfigurator<TClass,PropertyHolder<TClass,T>> 
            Foreign<TClass, T>(this NotifyConfigurator<TClass, PropertyHolder<TClass,T>> c,
            Expression<Func<TClass, int?>> idGetter
            )
            where TClass : NotifierBase,IEntity,IDataProvider
            where T : Entity<T>
        {
            var getter = idGetter.Compile();

            return c
                .TriggerExpression(idGetter)
                .On(e => e.DataService)
                .NotNull(e => e.DataService)
                .Set(e =>
                {
                    var id = getter(e);
                    return id == null ? null : e.DataService.FetchOne<T>(id.Value);
                })
            ;
        }

        public static NotifyConfigurator<TClass, PropertyHolder<TClass,ObservableQuery<T>>> Foreign<TClass, T>(this NotifyConfigurator<TClass, PropertyHolder<TClass,ObservableQuery<T>>> c,
            Expression<Func<T, int?>> idGetter
            )
            where TClass : Entity<TClass>,IDataProvider //NotifierBase,IEntity
            where T : Entity<T>
        {
            var getter = idGetter.Compile();

            return c;
            //    .TriggerExpression(idGetter)
            //    .Do((a, p) => p.Get().OnTriggered())
            //    .Do((a, p) => p.Get());
            // TODO :    .Set(e => new ObservableQuery<T>(e.Context.Db) { };
            //       .AddFilter(f => getter(f) == e.Id).FluentUpdate());
        }

    }
}