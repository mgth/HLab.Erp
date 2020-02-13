using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.EntityLists
{
    public class ListableEntityListViewModel<T> : EntityListViewModel<ListableEntityListViewModel<T>, T>
        where T : class, IEntity, IListableModel, new()
    {
        public ListableEntityListViewModel()
        {
            Columns
                .Icon("",e => e.IconPath)
                .Column("{Name}", e => e.Caption);

            List.UpdateAsync();
        }
        public ListableEntityListViewModel(Expression<Func<T,bool>> filter)
        {
            List.AddFilter(()=>filter);

            Columns
                .Icon("",e => e.IconPath)
                .Column("{Name}", e => e.Caption);

            List.UpdateAsync();
        }
    }

    public abstract class EntityListViewModel<TClass> : ViewModel<TClass>, IEntityListViewModel
    where TClass : EntityListViewModel<TClass>
    {
        public abstract void PopulateDataGrid(DataGrid grid);

        public abstract void SetOpenAction(Action<object> action);
        public abstract void SetSelectAction(Action<object> action);
        public ObservableCollection<IFilterViewModel> Filters { get; } = new ObservableCollection<IFilterViewModel>();
        public ICommand AddCommand { get; } = H.Command(c => c
                .Action(async e => await e.OnAddCommandAsync())
        );

        protected abstract Task OnAddCommandAsync();
    }

    public abstract class EntityListViewModel<TClass,T> : EntityListViewModel<TClass>, IEntityListViewModel<T>
    where TClass : EntityListViewModel<TClass, T>
        where T : class, IEntity, new()
    {
        public string Title => "{" + GetName() + "}";

        public string IconPath => "icons/entities/" + typeof(T).Name;

        private string GetName()
        {
            var n = GetType().Name;
            var i = n.IndexOf("ListViewModel");
            if (i >= 0) n = n.Substring(0, i);
            return n;
        }


        public new T Model
        {
            get => (T)base.Model;
            set => base.Model = value;
        }

        public override Type ModelType => typeof(T);

        
        [Import] private IDocumentService _docs;
        [Import] private IMessageBus _msg;
        [Import] private IDataService _data;

        
        [Import] public ObservableQuery<T> List { get; }
        public ColumnsProvider<T> Columns { get; }

        public ObservableCollection<dynamic> ListViewModel { get; } = new ObservableCollection<dynamic>();
        private readonly ConcurrentDictionary<T,dynamic> _cache = new ConcurrentDictionary<T, dynamic>();

        [Import] private Func<ObservableQuery<T>,ColumnsProvider<T>> _getColumnsProvider;

        protected EntityListViewModel()
        {
            Columns = _getColumnsProvider(List);
            List_CollectionChanged(null,new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add,List,0));
            List.CollectionChanged += List_CollectionChanged;
            H.Initialize((TClass)this,OnPropertyChanged);

            OpenAction = target => _docs.OpenDocumentAsync(target);
        }

        public ICommand OpenCommand { get; } = H.Command(c => c
            .CanExecute(e=>e.Selected!=null)
            .Action(e => e.OpenAction.Invoke(e.Selected))
            .On(e => e.Selected).CheckCanExecute()
        );

        protected Action<T> OpenAction;
        public override void SetOpenAction(Action<object> action)
        {
            OpenAction = action;
        }

        protected Action<T> SelectAction;
        public override void SetSelectAction(Action<object> action)
        {
            SelectAction = action;
        }




        [Import] protected Func<T> CreateInstance;

        protected override Task OnAddCommandAsync()
        {
            var entity = CreateInstance();

            if(entity is IEntity<int> e) e.Id=-1;
            return _docs.OpenDocumentAsync(entity);
        }

        public bool AddAllowed {get => _addAllowed.Get(); set => _addAllowed.Set(value);}
        private readonly IProperty<bool> _addAllowed = H.Property<bool>();

        public bool DeleteAllowed {get => _deleteAllowed.Get(); set => _deleteAllowed.Set(value);}
        private readonly IProperty<bool> _deleteAllowed = H.Property<bool>();

        private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newIndex = e.NewStartingIndex;
                    foreach (var n in e.NewItems.OfType<T>())
                    {
                        ObjectMapper<T> h = _cache.GetOrAdd(n,o => new ObjectMapper<T>(o, Columns));
                        ListViewModel.Insert(newIndex++,h);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<T>())
                    {
                        //TODO should keep deleted elements in cache for some times
                        if (_cache.TryRemove(item, out var h))
                            ListViewModel.Remove(h);
                        else
                        {
                            var mapper = ListViewModel.FirstOrDefault(x => (x is ObjectMapper<T> om) && Equals(om.Model.Id,item.Id));

                            if (mapper != null)
                                ListViewModel.Remove(mapper);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    ListViewModel.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public GridView View => _view.Get();
        private readonly IProperty<GridView> _view = H.Property<GridView>(c => c
            .Set(e => e.Columns.GetView()));
        public ListCollectionView ListCollectionView => _listCollectionView.Get();
        private readonly IProperty<ListCollectionView> _listCollectionView = H.Property<ListCollectionView>(c => c
            .Set(setter: e =>
            {
                var lcv = new ListCollectionView(e.ListViewModel);
                lcv.GroupDescriptions.Add(new PropertyGroupDescription("FileId"));
                return lcv;
            }));

        public override void PopulateDataGrid(DataGrid grid)
        {

            grid.SourceUpdated += delegate(object sender, DataTransferEventArgs args)
            {
                ICollectionView cv = CollectionViewSource.GetDefaultView(grid.ItemsSource);
                if (cv != null)
                {
                    cv.GroupDescriptions.Clear();
                    cv.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                }
            };


            foreach (var col in View.Columns)
            {
                grid.Columns.Add(new DataGridTemplateColumn
                {
                    Header = (col.Header as GridViewColumnHeader)?.Content,
                    CellTemplate = col.CellTemplate
                });
            }
        }

        public T Selected
        {
            get => _selected.Get();
            set {
                if (_selected.Set(value))
                {
                    if (SelectAction != null)
                        SelectAction(value);
                    else
                        _msg.Publish(new DetailMessage(value));
                }
            }
        }

        private readonly IProperty<T> _selected = H.Property<T>();

        public dynamic SelectedViewModel
        {
            get => _selectedViewModel.Get();
            set => Selected = value?.Model;
        }

        private readonly IProperty<dynamic> _selectedViewModel = H.Property<dynamic>(c => c
            .On(e => e.Selected)
            .Set(e => e.Selected==null?null:e._cache.GetOrAdd(e.Selected, o => new ObjectMapper<T>(o, e.Columns))));
    }
}
