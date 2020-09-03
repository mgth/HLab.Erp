using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    using H = H<EntityFilterViewModel>;

    public class EntityFilterViewModel : FilterViewModel, IEntityFilterViewModel
    {
        public EntityFilterViewModel() => H.Initialize(this);

        public INotifyCollectionChanged Query
        {
            get => _query.Get();   
            set
            {
                var old = Query;
                if (old != null)
                {
                    CollectionChanged(old, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    old.CollectionChanged -= CollectionChanged;
                }

                if (value != null)
                {
                    //value.AddFilter(Name, e => Match(Getter(e)));
                    CollectionChanged(old, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,value));
                    value.CollectionChanged += CollectionChanged;
                }

                _query.Set(value);
            }
        }
        private readonly IProperty<INotifyCollectionChanged> _query = H.Property<INotifyCollectionChanged>();

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var c = Getter((IEntity)item);
                        var counter = _count.AddOrUpdate(c, cc =>
                        {
                            Values.Add(cc);
                            return 1;
                        }, (cc,v)=>
                        {
                            if (v == 0) Values.Add(cc);
                            return v+1;
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var c = Getter((IEntity)item);
                        var counter = _count.AddOrUpdate(c, cc => 0, 
                            (cc, v) =>
                            {
                                if (v == 1) Values.Remove(cc);
                                return v-1;
                            });
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Values.Clear();
                    _count.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Func<IEntity, IEntity> Getter { get; set; }

        public bool Match(object id) => Value == null || Equals(Value.Id,id);

        private readonly ConcurrentDictionary<IEntity, int> _count = new ConcurrentDictionary<IEntity, int>();

        public ObservableCollection<IEntity> Values { get; } = new ObservableCollection<IEntity>();

        public IEntity Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<IEntity> _value = H.Property<IEntity>();
    }
}