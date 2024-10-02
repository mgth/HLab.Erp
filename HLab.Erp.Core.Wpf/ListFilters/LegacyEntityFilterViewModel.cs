namespace HLab.Erp.Core.ListFilters
{
    //

    //public class EntityFilterViewModel : FilterViewModel, IEntityFilterViewModel
    //{
    //    public EntityFilterViewModel() { }

    //    public INotifyCollectionChanged Query
    //    {
    //        get => _query;   
    //        set
    //        {
    //            var old = Query;
    //            if (old != null)
    //            {
    //                CollectionChanged(old, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //                old.CollectionChanged -= CollectionChanged;
    //            }

    //            if (value != null)
    //            {
    //                //value.AddFilter(Name, e => Match(Getter(e)));
    //                CollectionChanged(old, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,value));
    //                value.CollectionChanged += CollectionChanged;
    //            }

    //            _query.Set(value);
    //        }
    //    }
    //    private INotifyCollectionChanged _query ;

    //    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        switch (e.Action)
    //        {
    //            case NotifyCollectionChangedAction.Add:
    //                foreach (var item in e.NewItems)
    //                {
    //                    var c = Getter((IEntity)item);
    //                    var counter = _count.AddOrUpdate(c, cc =>
    //                    {
    //                        Values.Add(cc);
    //                        return 1;
    //                    }, (cc,v)=>
    //                    {
    //                        if (v == 0) Values.Add(cc);
    //                        return v+1;
    //                    });
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Remove:
    //                foreach (var item in e.OldItems)
    //                {
    //                    var c = Getter((IEntity)item);
    //                    var counter = _count.AddOrUpdate(c, cc => 0, 
    //                        (cc, v) =>
    //                        {
    //                            if (v == 1) Values.Remove(cc);
    //                            return v-1;
    //                        });
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Replace:
    //                break;
    //            case NotifyCollectionChangedAction.Move:
    //                break;
    //            case NotifyCollectionChangedAction.Reset:
    //                Values.Clear();
    //                _count.Clear();
    //                break;
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }
    //    }

    //    public Func<IEntity, IEntity> Getter { get; set; }

    //    public bool Match(object id) => Value == null || Equals(Value.Id,id);

    //    private readonly ConcurrentDictionary<IEntity, int> _count = new();

    //    public ObservableCollection<IEntity> Values { get; } = new();

    //    public IEntity Value
    //    {
    //        get => _value;
    //        set => this.SetAndRaise(ref _value,value);
    //    }
    //    private IEntity _value ;
    //}
}