using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using HLab.Core;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.Lists
{

    [Export(typeof(ViewModelsList))]
    public class ViewModelsList : ObservableCollection<IViewModel> // todo : test with ObservableHashset
    {
        [Import]
        private IMvvmService Mvvm { get; set; }
        [Import]
        private IMessageBus MessageBus { get; set; }



        private Type _viewMode;
        public Type ViewMode {
            get => _viewMode; set
        {
            if (_viewMode != value)
            {
                _viewMode = value;
                    
            };
        }
        }

        private readonly Action<INotifyPropertyChanged> _init = null;

        private INotifyCollectionChanged _source;
        public INotifyCollectionChanged Source
        {
            get => _source; set
            {
                ObservableCollection<IEntity> old;
                if (_source != null)
                {
                    _source.CollectionChanged -= _source_CollectionChanged;
                    old = new ObservableCollection<IEntity>((IEnumerable<IEntity>)_source);
                }
                else old = new ObservableCollection<IEntity>();

                _source = value;

                if(_source!=null)
                foreach (var item in (IEnumerable<IEntity>)_source)
                {
                    var vm = this.SingleOrDefault(tvm => ReferenceEquals(item, tvm.Model));
                    if (vm!=null)
                    {
                        
                        old.Remove(item);
                        //vm.Reload();
                    }
                    else
                    {
                        _source_CollectionChanged(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    }
                }

                foreach (var item in old)
                {
                        _source_CollectionChanged(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                }

                if (_source != null)
                    _source.CollectionChanged += _source_CollectionChanged;
            }
        }

        private readonly Action _reload = null;
        private readonly INotifyPropertyChanged _parent = null;
        public ViewModelsList(
            INotifyCollectionChanged source,
            INotifyPropertyChanged parent,
            Type viewMode = null, 
            Action reload=null,
            Action<INotifyPropertyChanged> init=null
            )
        {
            ViewMode = viewMode??typeof(ViewModeDefault);//::;
            _parent = parent;
            _reload = reload;
            _init = init;
            Source = source;

            MessageBus.Subscribe<EntityMessage>(UpdateList);
        }

        void UpdateList(EntityMessage msg)
//            where T : class, IEntity
        {
            foreach (var item in (IEnumerable<IEntity>)_source)
            {
                if ( msg.IsSameOrAnotherOf(item))
                {
                    _reload?.Invoke();
                    return;
                }
            }
        }

        private void _source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (Mvvm.MainContext.GetLinked(item, ViewMode, typeof(IViewClassDefault)) is IViewModel vm)
                    {
                        //vm.SetParent(_parent);
                        _init?.Invoke((INotifyPropertyChanged)vm);
                        Add(vm);                      
                    }
                        //new IViewModel { Model = item,ParentViewModel = _parent};
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<IEntity>())
                {
                    var del = this.SingleOrDefault(vm => ReferenceEquals(item, vm.Model));
                    if (del!=null) Remove(del);                    
                }
            }
        }
        //        public IQueryable<int> Query

    }


}
