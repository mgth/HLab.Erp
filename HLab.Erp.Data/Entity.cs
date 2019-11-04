
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using HLab.Base;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Data
{

    public abstract class Entity<T> : Entity<T,int>
        where T : Entity<T>
    {

        //protected Entity(ErpContext ctx) : base(ctx)
        //{
        //}
        //protected Entity()
        //{
        //}
    }

    public class ForeignProperty<T> : IForeign<T>
    where T:IEntity<int>
    {
        public ForeignProperty(IProperty<int?> id, IProperty<T> value)
        {
            Id = id;
            Value = value;
        }

        public IProperty<int?> Id { get; }
#if DEBUG
        public T Get([CallerMemberName]string name = null) => Value.Get(name);
#else        
        public T Get() => Value.Get();
#endif
        public void Set(T value) => Id.Set(value.Id);

        public IProperty<T> Value { get; }
        public void SetParent(object parent, INotifyClassParser parser, Action<PropertyChangedEventArgs> args)
        {
            Id.SetParent(parent,parser,args);
            Value.SetParent(parent,parser,args);
        }
    }


    [PrimaryKey("Id")]
    public abstract class Entity<TClass,T> : N<TClass>, IEntity<T>, IDataProvider//, IOnLoaded
        where TClass : Entity<TClass,T>
    {
        protected new class H : N<TClass>.H
        {
            public static IForeign<TF> Foreign<TF>([CallerMemberName] string name = null) where TF : Entity<TF>, IEntity<int>
            {
                name = H.Name(name) ;

                var property = typeof(TClass).GetProperty(name+ "Id");
                
                var parameter = Expression.Parameter(typeof(TClass), "e");
                var member = Expression.Property(parameter, property);
                var e = Expression.Lambda<Func<TClass, int?>>(member, parameter);

                return Foreign<TF>(e,name);
            }

            public static IForeign<TF> Foreign<TF>(Expression<Func<TClass,int?>> e,[CallerMemberName] string name = null) where TF : Entity<TF>, IEntity<int>
            {
                name = H.Name(name) ;
                var id = H.Property<int?>(name+"Id");
                var v = H.Property<TF>(c => c.Foreign(e),name);
                return new ForeignProperty<TF>(id,v);
            }
        }

        public virtual T Id
        {
            get => _id.Get();
            set => _id.Set(value);
        }
        private readonly IProperty<T> _id = H.Property<T>(c => c.Default((T)(object)-1));

        object IEntity.Id => Id;

        public virtual void OnLoaded()
        {
            IsLoaded = true;
        }

        [Ignore]
        public virtual bool IsLoaded { get; set; }
        protected Entity()
        {
            Initialize();
        }


        [Ignore]
        public IDataService DataService
        {
            get => _dataService.Get(); 
            set => _dataService.Set(value);
        }
        private readonly IProperty<IDataService> _dataService = H.Property<IDataService>();
    }
}