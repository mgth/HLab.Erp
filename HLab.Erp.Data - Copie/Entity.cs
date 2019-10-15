
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

    [Entity]
    public abstract class Entity<TClass,T> : N<TClass>, IEntity<T>//, IOnLoaded
        where TClass : Entity<TClass,T>
    {
        //[Ignore]
        //public IErpContext Context { get; }

//        protected Entity()
//        {
////            Context = ctx;
//            Initialize();
//        }

        [Column]
        public virtual T Id
        {
            get => _id.Get();
            set => _id.Set(value);
        }
        private readonly IProperty<T> _id = H.Property<T>();

        object IEntity.Id => Id;

        [Ignore]
        protected SuspenderToken Token;

        public virtual void OnLoaded()
        {
            //this.SubscribeNotifier();
            IsLoaded = true;
            //Token?.Dispose();
            //Token = null;
        }

        [Ignore]
        public virtual bool IsLoaded { get; set; }
    //}

    ////[ExplicitColumns]
    //public abstract class Entity<T> : Entity, IEntity
    //    where T : Entity<T>
    //{
        //protected Entity(ErpContext ctx)
        //{
        //    var c = RuntimeImportContext.GetStatic(null,GetType());

        //    ctx?.Container.Inject(this,null,c);

        //    N.Subscribe();
        //}
        protected Entity()
        {
            Initialize();
        }

        //protected override INotifier N
        //{
        //    get => base.N;
        //    set => base.N = value==null?null:new EntityHelper(value);
        //}

        //[NotMapped]
        //public EntityHelper E => (N as EntityHelper);

//        [Import]
//#pragma warning disable 649
//        [NotMapped] private readonly Func<Type, object> _locate;
//#pragma warning restore 649
//        protected TLocate Locate<TLocate>() => (TLocate)_locate(typeof(TLocate));


    }
}