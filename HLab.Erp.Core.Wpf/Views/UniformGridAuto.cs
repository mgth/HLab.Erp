using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Erp.Core.Wpf.Views;

public class UniformGridAuto : Grid
{
    protected override Size MeasureOverride(Size constraint)
    {
        var columns = Math.Max(1, Math.Round(constraint.Width / 500.0, 0));
        columns = Math.Min(Children.Count, columns);
        var rows = Math.Round(0.5 + Children.Count / columns, 0);

        ColumnDefinitions.Clear();
        RowDefinitions.Clear();
        RowDefinitions.Add(new() { Height = GridLength.Auto });

        for (var i = 0; i < columns; i++)
        {
            ColumnDefinitions.Add(new());
        }

        var c = 0;
        var r = 0;
        foreach (var e in Children.OfType<DependencyObject>())
        {
            e.SetValue(ColumnProperty, c);
            e.SetValue(RowProperty, r);

            c++;
            if (!(c >= columns)) continue;

            c = 0;
            r++;
            RowDefinitions.Add(new() { Height = GridLength.Auto });
        }
        return base.MeasureOverride(constraint);
    }
}