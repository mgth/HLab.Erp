
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using HLab.Notify.PropertyChanged;

using NPoco;

namespace HLab.Erp.Data
{

    public abstract class Entity : Entity<int>
    {
    }

    public class HD<TClass> : H<TClass> where TClass : NotifierBase, IEntity, IDataServiceProvider
    {
        public static IForeign<TF> Foreign<TF>([CallerMemberName] string name = null) where TF : Entity, IEntity<int>
        {
            var propertyName = GetNameFromCallerName(name);

            var property = typeof(TClass).GetProperty(propertyName + "Id");

            var parameter = Expression.Parameter(typeof(TClass), "e");
            var member = Expression.Property(parameter, property);
            var e = Expression.Lambda<Func<TClass, int?>>(member, parameter);

            return Foreign<TF>(e, name);
        }
        public static IForeign<TF> Foreign<TF>(Expression<Func<TClass, int?>> e, [CallerMemberName] string name = null) where TF : Entity, IEntity<int>
        {
            //var propertyName = GetNameFromCallerName(name) ;
            var id = Property<int?>(name + "Id");
            var v = Property<TF>(c => c.Foreign(e), name);
            return new ForeignProperty<TF>(id, v);
        }
    }


    [PrimaryKey("Id")]
    public abstract class Entity<T> : NotifierBase, IEntity<T>, IDataServiceProvider//, IOnLoaded
    where T : struct
    {

        public virtual T Id
        {
            get => _id.Get();
            set => _id.Set(value);
        }

        readonly IProperty<T> _id = H<Entity<T>>.Property<T>(c => c.Default((T)(object)-1));

        object IEntity.Id => Id;

        public virtual void OnLoaded()
        {
            IsLoaded = true;
        }

        [Ignore]
        [JsonIgnore]
        public virtual bool IsLoaded { get; set; }
        protected Entity() => H<Entity<T>>.Initialize(this);

        [Ignore]
        [JsonIgnore]
        public IDataService DataService
        {
            get => _dataService.Get();
            set => _dataService.Set(value);
        }

        readonly IProperty<IDataService> _dataService = H<Entity<T>>.Property<IDataService>();

    }
}