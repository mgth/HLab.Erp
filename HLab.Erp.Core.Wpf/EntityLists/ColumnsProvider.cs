using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Icons.Annotations.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.EntityLists
{


    public class ColumnsProvider<T> : IColumnsProvider<T> where T:class, IEntity
    {
        private readonly IIconService _icons;
         private readonly ILocalizationService _lang;

        private readonly Dictionary<string,IColumn<T>> _dict = new ();
        private readonly ObservableQuery<T> _list;

        public ColumnsProvider(ObservableQuery<T> list, IIconService icons, ILocalizationService lang)
        {
            _list = list;
            _icons = icons;
            _lang = lang;
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

        public object GetValue(T obj, string name)
        {
            if (_dict.ContainsKey(name))
            {
                return _dict[name].GetValue(obj);
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
                    Caption = column.Header,
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

                    _list.Update();
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
                if (column.Header is string s)
                    content = new Localize {Id = s};
                else
                {
                    content = column.Header;
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

                    _list.Update();
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

        public void AddColumn(IColumn<T> column)
        {
            _dict.Add(column.Id,column);
        }
    }
}