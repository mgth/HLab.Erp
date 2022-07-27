using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListView = System.Windows.Controls.ListView;

namespace HLab.Erp.Core.Wpf.EntityLists
{
    public class ColumnsProvider<T> : IColumnsProvider<T> where T:class, IEntity
    {
        readonly Dictionary<string,IColumn<T>> _dict = new ();
        public IObservableQuery<T> List {get;}

        public Dictionary<string,IColumn> Columns => _dict.ToDictionary(p=>p.Key,p=>(IColumn)p.Value);

        public ColumnsProvider(IObservableQuery<T> list)
        {
            List = list;
        }

        IColumn<T> _orderByColumn = null;

        void SetOrderBy(IColumn<T> column)
        {
            if (column == _orderByColumn) return;

            var next = column.OrderByNext;
            column.OrderByNext = _orderByColumn;
            _orderByColumn = column;
            var c = _orderByColumn;
            while (c.OrderByNext != null)
            {
                if (c.OrderByNext == column)
                {
                    c.OrderByNext = next;
                    return;
                }

                c = c.OrderByNext;
            }
        }

        public object GetValue(T obj, string name) => _dict.ContainsKey(name) ? _dict[name].GetValue(obj) : null;

        public void Populate(object grid)
        {
            if (grid is ListView {View: null} lv) lv.View = new GridView();

            foreach (var column in _dict.Values)
            {
                if (column.Hidden) continue;

                var header = new ColumnHeaderView {
                    DataContext = column,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    MinWidth = 30
                };

                header.SortDirectionChanged += (a, b) =>
                {
                    if (a is ColumnHeaderView h)
                    {
                        column.SortDirection = h.SortDirection;
                    }

                    SetOrderBy(column);
                    List.ResetOrderBy();
                    var c = _orderByColumn;
                    while (c != null)
                    {
                        List.AddOrderBy(c.OrderBy,c.SortDirection);
                        c = c.OrderByNext;
                    }

                    List.Update();
                };


                switch (grid)
                {
                    case DataGrid dataGrid:
                    {
                        var c = new DataGridTemplateColumn
                        {
                            Header = header,
                            //DisplayMemberBinding = new Binding(column.Id),
                            CellTemplate = CreateColumnTemplate(column.Id),
                            Width = column.Width,
                        };
                    
                        dataGrid.Columns.Add(c);
                        break;
                    }
                    case ListView {View : GridView gridView} :
                    {
                        var c = new GridViewColumn
                        {
                            Header = header,
                            Width = column.Width,
                            
                            //DisplayMemberBinding = new Binding(column.Id),
                            CellTemplate = CreateColumnTemplate(column.Id,column.Width),
                        };
                        gridView.Columns.Add(c);
                        c.Width = column.Width;
                        break;
                    }
                }
            }

        }


        //public object GetView()
        //{
        //    var gv = new GridView();

        //    foreach (var column in _dict.Values)
        //    {
        //        if (column.Hidden) continue;

        //        object content;
        //        if (column.Header is string s)
        //            content = new Localize {Id = s};
        //        else
        //        {
        //            content = column.Header;
        //        }

        //        var header = new GridViewColumnHeader 
        //        {
        //            Width = column.Width,
        //            Content = content
        //        };

        //        header.Click += (a, b) =>
        //        {
        //            SetOrderBy(column);
        //            List.ResetOrderBy();
        //            var c = _orderByColumn;
        //            while (c != null)
        //            {
        //                List.AddOrderBy(c.OrderBy,c.SortDirection);
        //            }

        //            List.Update();
        //        };
 
        //        var c = new GridViewColumn
        //        {
        //            Header = header,
        //            DisplayMemberBinding = new Binding(column.Id),
        //            //CellTemplate = CreateColumnTemplate(column.Id)
        //        };

        //        gv.Columns.Add(c);
        //    }

        //    return gv;
        //}

        public DataTemplate CreateColumnTemplate(string property,double width = 100.0)
        {
            var template = new DataTemplate();

            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(ContentControl));
            Binding binding = new(property)
            {
                Mode = System.Windows.Data.BindingMode.OneWay
            };

            //contentFactory.SetValue(ContentControl.WidthProperty, width); 
            //contentFactory.SetValue(ContentControl.ForegroundProperty, Brushes.White); //TODO : replace with themed color

            contentFactory.SetBinding(ContentControl.ContentProperty, binding);

            template.VisualTree = contentFactory;

            return template;


        //    StringReader stringReader = new StringReader(
        //        @$"<DataTemplate 
        //xmlns:mvvm=""clr-namespace:HLab.Mvvm;assembly=HLab.Mvvm""
        //xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
        //    <ContentControl Content=""{{Binding {property}}}""/> 
        //</DataTemplate>");


        //    XmlReader xmlReader = XmlReader.Create(stringReader);
        //    return XamlReader.Load(xmlReader) as DataTemplate;
        }

        public void AddColumn(IColumn<T> column)
        {
            _dict.Add(column.Id,column);
        }

        void RegisterTriggers(T model, Action<string> handler)
        {
            foreach(var column in _dict.Values)
            {
                column.RegisterTriggers(model,handler);
            }
        }

        void IColumnsProvider<T>.RegisterTriggers(T model, Action<string> handler)
        {
            RegisterTriggers(model, handler);
        }
    }
}