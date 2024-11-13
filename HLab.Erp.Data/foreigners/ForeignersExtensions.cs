using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReactiveUI;

namespace HLab.Erp.Data.foreigners;

public static class  ForeignersExtensions

{
    public static ForeignPropertyHelper<TObj, TRet> Foreign<TObj, TRet>(this TObj source, Expression<Func<TObj, int?>> idGetter, Expression<Func<TObj, TRet?>> getter)
        where TObj : Entity<int>, IReactiveObject 
        where TRet : class, IEntity<int>
    {
        var helper = source.WhenAnyValue(idGetter,s => s.DataService, (id,data) =>
        {
            return id == null ? default : data?.FetchOne<TRet>(e => e.Id == id);
        }).ToProperty(source, getter, deferSubscription: true);

        return new ForeignPropertyHelper<TObj, TRet>(source, helper);
    }
}