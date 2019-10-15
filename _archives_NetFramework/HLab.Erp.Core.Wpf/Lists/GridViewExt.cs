using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace HLab.Erp.Core.Wpf.Lists
{
    public static class GridViewExt
    {
        public static void AddBindings(this GridView gridView, Type type, string prefix = "")
        {
            foreach (var p in type.GetProperties())
            {
                foreach (var attr in p.GetCustomAttributes(false).OfType<IsListColumn>())
                {
                    var binding = new Binding(prefix + p.Name) {Mode = BindingMode.OneWay};

                    var col = new GridViewColumn
                    {
                        Header = attr.Header ?? p.Name,
                        DisplayMemberBinding = binding
                    };

                    gridView.Columns.Add(col);
                    gridView.SetValue(GridViewSort.PropertyNameProperty, p.Name);
                }
            }
        }
    }
}
