using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using HLab.Erp.Data.Observables;

namespace HLab.Erp.Data;

public static class PropertyEntityExtension
{
    //public static NotifyConfigurator<TClass,PropertyHolder<T>> 
    //    Foreign<TClass, T>(this NotifyConfigurator<TClass, PropertyHolder<T>> c,
    //    Expression<Func<TClass, int?>> idGetter
    //    )
    //    where TClass : NotifierBase,IEntity,IDataServiceProvider
    //    where T : Entity
    //{
    //    var getter = idGetter.Compile();



    //    return c
    //            .AddTriggerExpression(idGetter)
    //        .On(e => e.DataService)
    //        .NotNull(e => e.DataService)
    //        .Set(e =>
    //        {
    //            //TODO : have an async setter

    //            var id = getter(e);
    //            if (id == null) return default(T);

    //                var task = Task.Run(() => e.DataService.FetchOneAsync<T>(id.Value));
    //                task.Wait();
    //                var result = task.Result;
    //                return result;
    //        })
    //    ;
    //}

    //public static NotifyConfigurator<TClass, PropertyHolder<ObservableQuery<T>>> Foreign<TClass, T>(this NotifyConfigurator<TClass, PropertyHolder<ObservableQuery<T>>> c,
    //    Expression<Func<T, int?>> idGetter
    //    )
    //    where TClass : Entity,IDataServiceProvider //NotifierBase,IEntity
    //    where T : Entity
    //{
    //    var getter = idGetter.Compile();

    //    return c
    //        .Set(e => new ObservableQuery<T>(e.DataService)
    //           .AddFilter(() => f => getter(f) == e.Id).FluentUpdate());
    //}

}