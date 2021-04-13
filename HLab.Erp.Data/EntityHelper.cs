using System;
using System.Collections.Generic;
using System.Linq;

////using System.Data.Entity;

namespace HLab.Erp.Data
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public class EntityAttribute : Attribute
    { }

    //public class CustomWhenInjectedInto : ICompiledCondition
    //{
    //    private readonly Func<Type, bool> _typeTest;

    //    public CustomWhenInjectedInto(params Type[] types)
    //    {
    //        if (types == null) throw new ArgumentNullException(nameof(types));

    //        _typeTest = type => TestTypes(type, types);
    //    }

    //    /// <summary>
    //    /// Test if strategy meets condition at configuration time
    //    /// </summary>
    //    /// <param name="strategy">strategy to test</param>
    //    /// <param name="staticInjectionContext">static injection context</param>
    //    /// <returns>meets condition</returns>
    //    public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
    //    {
    //        var targetInfo =
    //            staticInjectionContext.InjectionStack.FirstOrDefault(
    //                info => info.RequestingStrategy?.StrategyType == ActivationStrategyType.ExportStrategy);

    //        var type = targetInfo?.InjectionType ??
    //                   staticInjectionContext.InjectionStack.LastOrDefault()?.LocateType;

    //        return type != null && _typeTest(type);
    //    }

    //    /// <summary>
    //    /// Tests for if one type is based on another
    //    /// </summary>
    //    /// <param name="injectionType"></param>
    //    /// <param name="types"></param>
    //    /// <returns></returns>
    //    protected bool TestTypes(Type injectionType, Type[] types)
    //    {
    //        foreach (var type in types)
    //        {
    //            if (ReflectionService.CheckTypeIsBasedOnAnotherType(injectionType, type))
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }
    //}
/*
    [Export(typeof(INotifier)), Decorator, WhenInjectedInto(typeof(IEntity))]
    public class EntityHelper : INotifier
    {
        [Import]
        public IDataService DbService { get; private set; }
        private readonly INotifier _notifier;
        public EntityHelper(INotifier notifier)
        {
            _notifier = notifier;
        }

        public T GetForeign<T>(Func<int?> getId, [CallerMemberName] string propertyName = null)
            where T : class, IEntity<int>
        {
            return Get<T>((old) =>
            {
                var id = getId();
                return (id == null) ? null : DbService.FetchOne<T>(id.Value);
            }, propertyName);

        }

        public void OnPropertyChanged(PropertyChangedEventArgs args) => _notifier.OnPropertyChanged(args);

        public bool Set<T>(T value, string propertyName = null, Action<T, T> postUpdateAction = null)
        {
            return _notifier.Set(value, propertyName, (old, v) =>
            {
                postUpdateAction?.Invoke(old,v);

                if ( Target is IEntity<int> entity   && entity.IsLoaded && entity.Id >= 0)
                {
                    if(entity.GetType().GetProperty(propertyName).GetCustomAttribute<NotMappedAttribute>() == null)
                    using (var db = (DbService as DataService).Get())
                    {
                        //db.Attach(entity).Property(propertyName).IsModified = true;
                       
                        //db.SaveChanges(); 
                            db.Update(entity, new[] { propertyName });
                    }
                }

            });
        }


        public bool AutoSave = false;

        public bool HasChanged => true;
        public Suspender Suspend => _notifier.Suspend;
        public void Add(PropertyChangedEventHandler value) => _notifier.Add(value);

        public void Remove(PropertyChangedEventHandler value) => _notifier.Remove(value);


        public T Get<T>(Func<T> getter, string propertyName = null) => _notifier.Get(getter, propertyName);
        public T Get<T>(Func<T, T> getter, string propertyName = null/, Action postUpdateAction = null/)
            => _notifier.Get(getter, propertyName/*, postUpdateAction/);

        public T Locate<T>(string propertyName = null) => _notifier.Locate<T>(propertyName);

        public T Locate<T>(Func<T, T> func, string propertyName = null) => _notifier.Locate<T>(func);

        public void Subscribe(PropertyChangedEventHandler handler, TriggerPath path) =>
            _notifier.Subscribe(handler, path);

        public void Subscribe(INotifierProperty triggeredProperty, INotifierProperty targetProperty,
            TriggerPath path) =>
            _notifier.Subscribe(triggeredProperty,targetProperty,path);

        public INotifierPropertyEntry GetPropertyEntry(string propertyName) =>
            _notifier.GetPropertyEntry(propertyName);

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(string propertyName) =>
            _notifier.GetPropertyEntry<T>(propertyName);

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(PropertyInfo propertyInfo) =>
            _notifier.GetPropertyEntry<T>(propertyInfo);

        public INotifierPropertyEntry GetPropertyEntry(INotifierProperty property) =>
            _notifier.GetPropertyEntry(property);

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(INotifierProperty<T> property) =>
            _notifier.GetPropertyEntry<T>(property);

        public INotifier Subscribe() => _notifier.Subscribe();

        public bool SetOneToMany<TClass, T>(T value, Func<T, IList<TClass>> getCollection, [CallerMemberName] string propertyName = null)
        => _notifier.SetOneToMany(value, getCollection, propertyName);


        //public bool SetOneToMany<TNotifier, T>(T value, Func<T, IList<TNotifier>> getCollection,
        //    string propertyName) where TNotifier : INotifierObject
        //    => _notifier.SetOneToMany(value, getCollection, propertyName);

        public bool Subscribed => _notifier.Subscribed;
        public INotifyPropertyChanged Target => _notifier.Target;
    }
*/




    public interface IOrderedEntity
    {
        int? Order { get; set; }
    }

    public static class OrderedEntityNotifierExt
    {
        public static void PlaceLast<T>(this T entity, IEnumerable<T> db) where T : class, IOrderedEntity
        {
            entity.Order = (db.Max(x => x.Order) ?? -1) + 1;
        }
        public static void PlaceAt<T>(this IOrderedEntity entity, IEnumerable<T> db, int position) where T : class, IOrderedEntity
        {
            entity.Unplace(db);
            foreach (var others in db.Where(x => x.Order >= position))
            {
                others.Order++;
            }
            entity.Order = position;
        }

        public static void Unplace<T>(this T entity, IEnumerable<T> db) where T : class, IOrderedEntity
        {
            if (entity.Order == null) return;
            int old = entity.Order.Value;
            entity.Order = null;
            foreach (var others in db.Where(x => x.Order > old))
            {
                others.Order--;
            }
        }


    }
}
