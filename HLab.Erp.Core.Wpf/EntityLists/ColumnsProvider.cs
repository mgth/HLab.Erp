using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Icons.Annotations.Icons;
using HLab.Localization.Wpf.Lang;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListView = System.Windows.Controls.ListView;

namespace HLab.Erp.Core.EntityLists
{
    [Export(typeof(IColumnsProvider<>))]
    public class ColumnsProvider<T> : IColumnsProvider<T> where T:class, IEntity
    {
        private readonly Dictionary<string,IColumn<T>> _dict = new ();
        private readonly ObservableQuery<T> _list;

        public ColumnsProvider(ObservableQuery<T> list)
        {
            _list = list;
        }

        public object GetValue(T obj, string name) => _dict.ContainsKey(name) ? _dict[name].GetValue(obj) : null;

        public void Populate(object grid)
        {
            if (grid is ListView {View: null} lv) lv.View = new GridView();

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
                        
                            CellTemplate = CreateColumnTemplate(column.Id),
                        };
                        gridView.Columns.Add(c);
                        c.Width = column.Width;

                        break;
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