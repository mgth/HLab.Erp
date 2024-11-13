using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using HLab.Base.ReactiveUI;
using HLab.Erp.Data.foreigners;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Data;

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
        set => this.SetAndRaise(ref _id, value);
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


    [Ignore]
    [JsonIgnore]
    public IDataService DataService
    {
        get => _dataService;
        set => this.RaiseAndSetIfChanged(ref _dataService, value);
    }
    IDataService _dataService;

}