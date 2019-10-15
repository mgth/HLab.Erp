using System;
using System.ComponentModel;
using System.Linq;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Lists.QueryLists
{
    public class QueryList<T, TVm> : QueryListBase<T, TVm, QueryList<T, TVm>>
        where TVm : IViewModel<T>
        where T : class, INotifyPropertyChanged, IEntity, new()
    {
        public QueryList(INotifyPropertyChanged parentViewModel, Func<IQueryable<T>> sourceFunc, Type viewMode) : base(parentViewModel, sourceFunc, viewMode)
        {
        }
    }

    [Obsolete]
    public class QueryList<T> : QueryListBase<T, IViewModel<T>, QueryList<T>>
        where T : class, IEntity, new()
    {
        public QueryList(INotifyPropertyChanged parentViewModel, Func<IQueryable<T>> sourceFunc, Type viewMode) : base(parentViewModel, sourceFunc, viewMode)
        {
        }
    }
    public class EntityQueryList<T, TVm> : QueryListBase<T, TVm, EntityQueryList<T, TVm>>
        where T : class, IEntity, new()
        where TVm : IViewModel<T>
    {

    public EntityQueryList(INotifyPropertyChanged parentViewModel, Type viewMode, IDbService db)
        : base(parentViewModel, () => db.Get().Query<T>(), viewMode)
    {
    }
    public EntityQueryList(INotifyPropertyChanged parentViewModel, Func<IQueryable<T>> sourceFunc, Type viewMode)
        : base(parentViewModel, sourceFunc, viewMode)
    {
    }

        //public EntityQueryList<T, TVm> OneToMany<T2>(Func<T2> oneFunc, Expression<Func<T, T2>> many)
        //    where T2 : IEntity<T2>
        //{
        //    return (EntityQueryList<T, TVm>)AddFilter(
        //        e =>
        //        {
        //            var one = oneFunc()?.Id;
        //            return e.Include(many).Where(
        //                t => one != null
        //                     && many.Compile()(t) != null
        //                     && one == many.Compile()(t).Id
        //            );
        //        }, -1, "OneToMany");
        //}

        //       public EntityQueryList<T, TVm> OneToMany(Func<int> oneFunc, Expression<Func<T, int?>> many)
        public EntityQueryList<T, TVm> OneToMany(Func<int> oneFunc, Func<T, int?> many)
        {
            return (EntityQueryList<T, TVm>)AddFilter(
                e =>
                {
                    var one = oneFunc();
                    return e.Where(
                        t => one == many(t)
                    );
                }, -1, "OneToMany");
        }
    }
}
