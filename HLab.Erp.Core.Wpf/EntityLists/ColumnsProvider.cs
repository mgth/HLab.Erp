using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Core.EntityLists
{
    public class ColumnsProvider<T> where T:class, IEntity
    {
        [Import]
        private IIconService _icons;
        [Import]
        private ILocalizationService _lang;

        private readonly Dictionary<string,Column<T>> _dict = new Dictionary<string, Column<T>>();
        private ObservableQuery<T> _list;

        public ColumnsProvider(ObservableQuery<T> list)
        {
            _list = list;
        }

        public ColumnsProvider<T> Icon(string caption, Func<T,string> iconPath,Func<T,object> orderBy=null,double height=25.0, string id=null)
        {
            return ColumnAsync(caption, async (s) =>
            {
                var icon = await _icons.GetIconAsync(iconPath(s));
                if (icon is FrameworkElement fe) fe.MaxHeight = height;
                return icon;

            }, orderBy);
        }
        public ColumnsProvider<T> Localize(string caption, Func<T,string> text,Func<T,object> orderBy=null,double height=25.0, string id=null)
        {
            return ColumnAsync(caption, async (s) =>
            {

                var localized = await _lang.LocalizeAsync(text(s));
                return new Label { Content = localized };

            }, orderBy);
        }

        public ColumnsProvider<T> ColumnAsync(string caption, Func<T,Task<object>> f,Func<T,object> orderBy,string id=null)
        {
            var c = new Column<T>(caption,t => new AsyncView{Getter = async () => await f(t)} ,orderBy, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        public ColumnsProvider<T> Column(string caption, Func<T,object> f,string id=null)
        {
            var c = new Column<T>(caption,f,f, id, false);
            _dict.Add(c.Id,c);
            return this;
        }
        public ColumnsProvider<T> Column(string caption,Func<T,object> getter, Func<T,object> orderBy,string id=null)
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
        public ColumnsProvider<T> Hidden(string id, Func<T,object> f)
        {
            var c = new Column<T>("", f,null, id, true);
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

        public void PopulateGridView(DataGrid grid)
        {
            foreach (var column in _dict.Values)
            {
                if (column.Hidden) continue;

                object content;
                if (column.Caption is string s)
                    content = new Localize { Id = s };
                else
                {
                    content = column.Caption;
                }

                var header = new Button {
                    Content = content,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };
                header.Click += (a, b) =>
                {
                    //TODO : click is never called and so sorting does not work 
                    _list.OrderBy = column.OrderBy;
                    _list.UpdateAsync();
                };

                var c = new DataGridTemplateColumn
                {
                    Header = header,
                    //DisplayMemberBinding = new Binding(column.Id),
                    CellTemplate = CreateColumnTemplate(column.Id),
                };

                grid.Columns.Add(c);
            }

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

                var header = new GridViewColumnHeader {Content = content};
                header.Click += (a, b) =>
                {
                    //TODO : click is never called and so sorting does not work 
                    _list.OrderBy = column.OrderBy;
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
}