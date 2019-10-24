using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Lang;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ViewModels
{
    public abstract class EntityListViewModel<TClass> : ViewModel<TClass>, IListViewModel
    where TClass : EntityListViewModel<TClass>
    {
        public virtual void PopulateDataGrid(DataGrid grid)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class EntityListViewModel<TClass,T> : EntityListViewModel<TClass>
    where TClass : EntityListViewModel<TClass, T>
        where T : class, IEntity, new()
    {
        public new T Model
        {
            get => (T)base.Model;
            set => base.Model = value;
        }

        public override Type ModelType => typeof(T);

        [Import]
        private IDocumentService _docs;
        [Import]
        private IMessageBus _msg;

        [Import]
        public ObservableQuery<T> List { get; }
        public ColumnsProvider<T> Columns { get; } = new ColumnsProvider<T>();
        public ObservableCollection<IFilterViewModel> Filters { get; } = new ObservableCollection<IFilterViewModel>();

        public ObservableCollection<dynamic> ListViewModel { get; } = new ObservableCollection<dynamic>();
        private readonly ConcurrentDictionary<T,dynamic> _cache = new ConcurrentDictionary<T, dynamic>();
        protected EntityListViewModel()
        {
            List.CollectionChanged += List_CollectionChanged;
            H.Initialize((TClass)this,OnPropertyChanged);
        }

        public ICommand OpenCommand { get; } = H.Command(c => c
            .CanExecute(e=>e.Selected!=null)
            .Action(async e => await e.OnOpenCommand(e.Selected))
            .On(e => e.Selected).CheckCanExecute()
        );

        protected virtual async Task OnOpenCommand(T target)
        {
            await _docs.OpenDocument(target);
        }
        public ICommand AddCommand { get; } = H.Command(c => c
//            .CanExecute(e=>e.Selected!=null)
            .Action(async e => await e.OnAddCommand(e.Selected))
//            .On(e => e.Selected).CheckCanExecute()
        );
        protected virtual async Task OnAddCommand(T target)
        {
            await _docs.OpenDocument(target);
        }

        private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var n in e.NewItems.OfType<T>())
                    {
                        ObjectMapper<T> h = _cache.GetOrAdd(n,o => new ObjectMapper<T>(o, Columns));
                        ListViewModel.Add(h);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var n in e.OldItems.OfType<T>())
                    {
                        if(_cache.TryRemove(n,out var h))
                            ListViewModel.Remove(h);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();
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
            set { if(_selected.Set(value)) _msg.Publish(new DetailMessage(value)); }
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

    public class Column<T>
    {
        public Column(object caption, Func<T, object> getter, string id, bool hidden)
        {
            Id = id ?? ("C" + Guid.NewGuid().ToString().Replace('-', '_'));
            Caption = caption;
            _getter = getter;
            Hidden = hidden;
        }

        public bool Hidden { get; }
        public object Caption { get; }
        private readonly Func<T, object> _getter;

        public object Get(T value)
        {
            return _getter(value);
        }

        public string Id { get; }
    }

    public class ColumnsProvider<T>
    {
        private readonly Dictionary<string,Column<T>> _dict = new Dictionary<string, Column<T>>();

        public ColumnsProvider<T> Column(string caption, Func<T,Task<object>> f,string id=null)
        {
            var c = new Column<T>(caption,t => new AsyncView{Getter = async () => await f(t)} , id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        public ColumnsProvider<T> Column(string caption, Func<T,object> f,string id=null)
        {
            var c = new Column<T>(caption,f, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        //public ColumnsProvider<T> Hidden(string id, Func<T,Task<object>> f)
        //{
        //    var c = new Column<T>("", f, id, true);
        //    _dict.Add(c.Id,c);
        //    return this;
        //}
        public ColumnsProvider<T> Hidden(string id, Func<T,object> f)
        {
            var c = new Column<T>("", f, id, true);
            _dict.Add(c.Id,c);
            return this;
        }

        public object GetValue(T obj, string name)
        {
            if (_dict.ContainsKey(name))
            {
                return _dict[name].Get(obj);
            }
            else return null;
        }

        public GridView GetView()
        {
            var gv = new GridView();

            foreach (var column in _dict.Values)
            {
                if (column.Hidden) continue;

                object content;
                if (column.Caption is string s)
                    content = new Localize {Id = s};
                else
                {
                    content = column.Caption;
                }

                var c = new GridViewColumn
                {
                    Header = new GridViewColumnHeader { Content = content },
                    //DisplayMemberBinding = new Binding(column.Id),
                    CellTemplate = CreateColumnTemplate(column.Id)
                };

                gv.Columns.Add(c);
            }

            return gv;
        }
        public DataTemplate CreateColumnTemplate(string property)
        {
        //    StringReader stringReader = new StringReader(
        //        @"<DataTemplate 
        //xmlns:mvvm=""clr-namespace:HLab.Mvvm;assembly=HLab.Mvvm""
        //xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
        //    <mvvm:ViewLocator Model=""{Binding " + property + @"}""/> 
        //</DataTemplate>");


            StringReader stringReader = new StringReader(
                @"<DataTemplate 
        xmlns:mvvm=""clr-namespace:HLab.Mvvm;assembly=HLab.Mvvm""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
            <ContentControl Content=""{Binding " + property + @"}""/> 
        </DataTemplate>");


            XmlReader xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader) as DataTemplate;
        }
    }

    public sealed class ObjectMapper<T> : DynamicObject
    {
        private readonly T _model;
        private readonly ColumnsProvider<T> _columns;

        public T Model => _model;

        public ObjectMapper(T model, ColumnsProvider<T> columns)
        {
            _model = model;
            _columns = columns;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch (binder.Name)
            {
                //case "Model":
                //    result = _model;
                //    return true;
                default:
                    result = _columns.GetValue(_model, binder.Name);
                    return true;
            }
        }
    }
}
