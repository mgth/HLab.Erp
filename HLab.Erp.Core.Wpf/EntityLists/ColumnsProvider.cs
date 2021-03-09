using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{
    public static class ColumnConfiguratorExtension
    {
        public static IColumnConfigurator<T> Icon<T>(this IColumnConfigurator<T> c, Func<T, string> getPath, double maxHeight = 30.0)
        {
            var getContent = c.Current.Getter;
            if(getContent==null)
                return c.Content(t => new IconView {Path = getPath(t), IconMaxHeight = maxHeight});
            
            return c.Content(t => new IconView {Path = getPath(t), IconMaxHeight = maxHeight, Caption = getContent(t)});
        }

        public static IColumnConfigurator<T> Localize<T>(this IColumnConfigurator<T> c)
        {
            var getContent = c.Current.Getter;
            return c.Content( e=> new Localize {Id=(string)getContent(e)});
        }

        public static IColumnConfigurator<T> Center<T>(this IColumnConfigurator<T> c)
        {
            var getContent = c.Current.Getter;
            
            return c.Content(t => new ContentControl {VerticalAlignment = VerticalAlignment.Stretch, Content = getContent?.Invoke(t)});
        }


    }

    public class ColumnConfigurator<T> : IColumnConfigurator<T>, IDisposable where T : class, IEntity
    {
        public IColumn<T> Current { get; private set; }

        //private Expression<Func<T, object>> _getter;

        private readonly Dictionary<string, Column<T>> _dict;
//        private readonly IEntityListViewModel<T> _list;

        public ColumnConfigurator(Dictionary<string, Column<T>> dict/*, IEntityListViewModel<T> list*/)
        {
            _dict = dict;
//            _list = list;
        }

        public IColumnConfigurator<T> Header(object caption)
        {
            Current.Caption = caption;
            return this;
        }

        public IColumnConfigurator<T> Width(double width)
        {
            Current.Width = width;
            return this;
        }

        public IColumnConfigurator<T> Id(string id)
        {
            Current.Id = id;
            return this;
        }

        public IColumnConfigurator<T> Content(Func<T, Task<object>> getContent)
        {
            Current.Getter = t => new AsyncView{Getter = async () => await getContent(t)};
            return this;
        }

        public IColumnConfigurator<T> OrderBy(Func<T,object> orderBy)
        {
            Current.OrderBy = orderBy;
            return this;
        }
        public IColumnConfigurator<T> OrderByOrder(int order)
        {
            foreach (var column in _dict.Values.Where(c => c.OrderByOrder>=order).ToArray())
            {
                column.OrderByOrder++;
            }
            Current.OrderByOrder = order;
            return this;
        }

        public IColumnConfigurator<T> Hidden
        {
            get
            {
                Current.Hidden = true;
                return this;
            }
        }

        public IColumnConfigurator<T> Content(Func<T,object> getter)
        {
                Current.Getter = getter;
                if (Current.OrderBy == null)
                {
                    Current.OrderBy = getter;
                }
                return this;
        }

        public IColumnConfigurator<T> Mvvm<TViewClass>() where TViewClass : IViewClass
        {
            Current.Getter = o => new ViewLocator{ViewClass = typeof(TViewClass), DataContext = o};
            return this;
        }

        public IColumnConfigurator<T> Column
        {
            get
            {
                var order = 0;
                if (Current != null)
                {
                    Add();
                    order = _dict.Values.Max(c => c.OrderByOrder)+1;
                }
                Current = new Column<T> {OrderByOrder = order};
                return this;
            }
        }

        public IColumnConfigurator<T> Filter<TF>() where TF : IFilterViewModel, new()
        {
//            _filter = new TF();
             return this;
        }

        private void Add()
        {
            Debug.Assert(Current.Getter!=null);
            _dict.Add(Current.Id, (Column<T>) Current);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Add();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


    public class ColumnsProvider<T> : IColumnsProvider<T> where T:class, IEntity
    {
        [Import]
        private IIconService _icons;
        [Import]
        private ILocalizationService _lang;

        private readonly Dictionary<string,Column<T>> _dict = new ();
        private readonly ObservableQuery<T> _list;

        public ColumnsProvider(ObservableQuery<T> list)
        {
            _list = list;
        }

        public IColumnsProvider<T> Icon(string caption, Func<T,string> iconPath,Func<T,object> orderBy=null,double height=25.0, string id=null)
        {
            return ColumnAsync(caption, async (s) =>
            {
                var icon = await _icons.GetIconAsync(iconPath(s));
                if (icon is FrameworkElement fe) fe.MaxHeight = height;
                return icon;

            }, orderBy);
        }
        public IColumnsProvider<T> Localize(string caption, Func<T,string> text,Func<T,object> orderBy=null,double height=25.0, string id=null)
        {
            return ColumnAsync(caption, async (s) =>
            {

                var localized = await _lang.LocalizeAsync(text(s));
                return new Label { Content = localized };

            }, orderBy);
        }

        public IColumnsProvider<T> ColumnAsync(string caption, Func<T,Task<object>> f,Func<T,object> orderBy,string id=null)
        {
            var c = new Column<T>(caption,t => new AsyncView{Getter = async () => await f(t)} ,orderBy, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        public IColumnsProvider<T> Column(string caption, Func<T,object> f,string id=null)
        {
            var c = new Column<T>(caption,f,f, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        public IColumnsProvider<T> Column(string caption,Func<T,object> getter, Func<T,object> orderBy,string id=null)
        {
            var c = new Column<T>(caption,getter,orderBy, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        //public ColumnsProvider<T> Hidden(string id, Func<T,Task<object>> f)
        //{
        //    var c = new Column<T>("", f, id, true);
        //    _dict.Add(c.Id,c);
        //    return this;
        //}
        public IColumnsProvider<T> Hidden(string id, Func<T,object> f)
        {
            var c = new Column<T>("", f,null, id, true);
            _dict.Add(c.Id,c);
            return this;
        }

        public void Configure(Func<IColumnConfigurator<T>,IColumnConfigurator<T>> f)
        {
            using (var configurator = new ColumnConfigurator<T>(_dict))
            {
                f(configurator);
            }
        }

        public object GetValue(T obj, string name)
        {
            if (_dict.ContainsKey(name))
            {
                return _dict[name].Get(obj);
            }
            else return null;
        }

        public void Populate(object grid)
        {
            if (grid is ListView lv && lv.View == null) lv.View = new GridView();


            foreach (var column in _dict.Values)
            {
                if (column.Hidden) continue;


                var header = new ColumnHeaderView {
                    Caption = column.Caption,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    MinWidth = 30
                };

                header.SortDirectionChanged += (a, b) =>
                {
                    foreach (var c in _dict.Values.Where(c => c.OrderByOrder<column.OrderByOrder).ToArray())
                    {
                        c.OrderByOrder++;
                    }
                    column.OrderByOrder = 0;
                    _list.ResetOrderBy();
                    foreach (var c in _dict.Values.OrderBy(e => e.OrderByOrder))
                    {
                        _list.AddOrderBy(c.OrderBy,c.OrderDescending,c.OrderByOrder);
                    }

                    _list.UpdateAsync();
                };


                if (grid is DataGrid dataGrid)
                {
                    var c = new DataGridTemplateColumn
                    {
                        Header = header,
                        //DisplayMemberBinding = new Binding(column.Id),
                        CellTemplate = CreateColumnTemplate(column.Id),
                        Width = column.Width,
                    };
                    
                    dataGrid.Columns.Add(c);
                }

                else if (grid is ListView listView)
                {

                    if (listView.View is GridView gridView)
                    {
                        

                        var c = new GridViewColumn
                        {
                            Header = header,
                            Width = column.Width,
                            
                            //DisplayMemberBinding = new Binding(column.Id),
                            CellTemplate = CreateColumnTemplate(column.Id),
                        };
                        gridView.Columns.Add(c);
                        c.Width = column.Width;
                    }
                }
            }

        }


        public object GetView()
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

                var header = new GridViewColumnHeader 
                {
                    Width = column.Width,
                    Content = content
                };

                header.Click += (a, b) =>
                {
                    foreach (var col in _dict.Values)
                    {
                        if (col.OrderByOrder < column.OrderByOrder) col.OrderByOrder++;
                    }
                    column.OrderByOrder = 0;
                    foreach (var col in _dict.Values.OrderByDescending(e => e.OrderByOrder))
                    {
                        _list.AddOrderBy(col.OrderBy,col.OrderDescending);
                    }

                    _list.UpdateAsync();
                };
 
                var c = new GridViewColumn
                {
                    Header = header,
                    //DisplayMemberBinding = new Binding(column.Id),
                    CellTemplate = CreateColumnTemplate(column.Id)
                };

                gv.Columns.Add(c);
            }

            return gv;
        }
        public DataTemplate CreateColumnTemplate(string property)
        {

            var t = new DataTemplate();

            FrameworkElementFactory cc = new FrameworkElementFactory(typeof(ContentControl));
            cc.SetBinding(ContentControl.ContentProperty,new Binding(property));
            cc.SetValue(ContentControl.VerticalAlignmentProperty,VerticalAlignment.Top);
            cc.SetValue(ContentControl.VerticalContentAlignmentProperty,VerticalAlignment.Top);
            t.VisualTree = cc;

            return t;

            //    StringReader stringReader = new StringReader(
            //        @"<DataTemplate 
            //xmlns:mvvm=""clr-namespace:HLab.Mvvm;assembly=HLab.Mvvm""
            //xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
            //    <mvvm:ViewLocator Model=""{Binding " + property + @"}""/> 
            //</DataTemplate>");


            StringReader stringReader = new StringReader(
                @$"<DataTemplate 
        xmlns:mvvm=""clr-namespace:HLab.Mvvm;assembly=HLab.Mvvm""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
            <ContentControl Content=""{{Binding {property}}}""/> 
        </DataTemplate>");


            XmlReader xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader) as DataTemplate;
        }
    }
}