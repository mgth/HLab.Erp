
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using NPoco;
using ReactiveUI;

namespace HLab.Erp.Data;

public abstract class Entity : Entity<int>
{
}
public interface INotifyDataHelper<TClass> where TClass : ReactiveObject, IEntity, IDataServiceProvider
{
//    public H<TClass> H => HD<TClass>.Helper;
}

//public class HD<TClass> : H<TClass> where TClass : ReactiveObject, IEntity, IDataServiceProvider
//{
//    public new static HD<TClass> Helper => new();

    //public static IForeign<TF> Foreign<TF>([CallerMemberName] string name = null) where TF : Entity, IEntity<int>
    //{
    //    var propertyName = GetNameFromCallerName(name);

    //    var property = typeof(TClass).GetProperty(propertyName + "Id");

    //    var parameter = Expression.Parameter(typeof(TClass), "e");
    //    var member = Expression.Property(parameter, property);
    //    var e = Expression.Lambda<Func<TClass, int?>>(member, parameter);

    //    return Foreign<TF>(e, name);
    //}
    //public static IForeign<TF> Foreign<TF>(Expression<Func<TClass, int?>> e, [CallerMemberName] string name = null) where TF : Entity, IEntity<int>
    //{
    //    //var propertyName = GetNameFromCallerName(name) ;
    //    var id = Property<int?>(name + "Id");
    //    var v = Property<TF>(c => c.Foreign(e), name);
    //    return new ForeignProperty<TF>(id, v);
    //}
//}


[PrimaryKey("Id")]
public abstract class Entity<T> : ReactiveObject, IEntity<T>, IDataServiceProvider//, IOnLoaded
where T : struct
{

    public virtual T Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    T _id = (T)(object)-1;


    object IEntity.Id => Id;

    public virtual void OnLoaded()
    {
        IsLoaded = true;
    }

    [Ignore]
    [JsonIgnore]
    public virtual bool IsLoaded { get; set; }

    protected ObservableAsPropertyHelper<TRet> Foreign<TObj,TRet>(TObj source,Expression<Func<TObj,int?>> idGetter,Expression<Func<TObj,TRet>> getter) 
        where TObj : class, IReactiveObject
        where TRet : Entity
    {
            return source.WhenAnyValue(idGetter, id =>
            {
                if (id == null) return (TRet)null;
                return DataService.FetchOne<TRet>(e => e.Id == id);
            }).ToProperty(source, getter, deferSubscription: true);

    }


    [Ignore]
    [JsonIgnore]
    public IDataService DataService
    {
        get => _dataService;
        set => this.RaiseAndSetIfChanged(ref _dataService, value);
    }
    IDataService _dataService;

}