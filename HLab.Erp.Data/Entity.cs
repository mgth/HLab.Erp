using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using HLab.Base.ReactiveUI;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Data;

public class ForeignPropertyHelper<TObj, TRet>(TObj parent, ObservableAsPropertyHelper<TRet> foreign)
    where TObj : class, IReactiveObject
    where TRet : class, IEntity<int>
{
    int? _id;

    public int? Id => _id;
    public TRet Value => foreign.Value;

    public void SetId(int? id, [CallerMemberName] string? name = null)
    {
        parent.RaiseAndSetIfChanged(ref _id, id, name);
    }
}


public abstract class Entity : Entity<int>
{
}
public interface INotifyDataHelper<TClass> where TClass : ReactiveObject, IEntity, IDataServiceProvider
{
}

[PrimaryKey("Id")]
public abstract class Entity<T> : ReactiveModel, IEntity<T>, IDataServiceProvider//, IOnLoaded
where T : struct
{

    public virtual T Id
    {
        get => _id;
        set => SetAndRaise(ref _id, value);
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

    protected ForeignPropertyHelper<TObj, TRet> Foreign<TObj, TRet>(TObj source, Expression<Func<TObj, int?>> idGetter, Expression<Func<TObj, TRet?>> getter)
        where TObj : class, IReactiveObject
        where TRet : class, IEntity<int>
    {
        var helper = source.WhenAnyValue(idGetter, id =>
        {
            return id == null ? default : DataService.FetchOne<TRet>(e => e.Id == id);
        }).ToProperty(source, getter, deferSubscription: true);

        return new ForeignPropertyHelper<TObj, TRet>(source, helper);
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