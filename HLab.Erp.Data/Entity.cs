
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

    [PrimaryKey("Id")]
    public abstract class Entity<TClass,T> : N<TClass>, IEntity<T>, IDataProvider//, IOnLoaded
        where TClass : Entity<TClass,T>
    {

        public virtual T Id
        {
            get => _id.Get();
            set => _id.Set(value);
        }
        private readonly IProperty<T> _id = H.Property<T>();

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