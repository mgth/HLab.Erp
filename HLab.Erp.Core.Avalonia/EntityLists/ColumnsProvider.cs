using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using HLab.Base;
using HLab.Base.Extensions;
using HLab.Erp.Core.EntityLists;
// TriggerPath vit dans le projet partagé HLab.Erp.Core mais sous un namespace Wpf (résidu)
using TriggerPath = HLab.Erp.Core.Wpf.EntityLists.TriggerPath;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;

namespace HLab.Erp.Core.Avalonia.EntityLists;

public class ColumnsProvider<T>(IObservableQuery<T> list) : IColumnsProvider<T>
    where T : class, IEntity
{
    readonly Dictionary<string, IColumn<T>> _dict = new();
    public IObservableQuery<T> List { get; } = list;

    public Dictionary<string, IColumn> Columns => _dict.ToDictionary(p => p.Key, p => (IColumn)p.Value);

    IColumn<T>? _orderByColumn = null;

    void SetOrderBy(IColumn<T> column, int rank = 0)
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

    public bool GetValue(T obj, string name, out object? result)
    {
        if (!_properties.TryGetValue(name, out var property))
        {
            result = null;
            return false;
        }

        try
        {
            result = property.CanEvaluate(obj) ? property.Getter?.Invoke(obj) : null;
            return true;
        }
        catch (NullReferenceException)
        {
            result = null;
            return true;
        }
    }

    IColumn<T>? GetColumnAtOrderRank(int rank, ref int nextRank)
    {
        var maxRank = -1;

        foreach (var c in _dict.Values)
        {
            var r = c.OrderByRank;

            if (r > maxRank) maxRank = c.OrderByRank;

            if (c.OrderByRank != rank) continue;

            nextRank--;

            return c;
        }

        nextRank = maxRank;
        return null;
    }

    public void SetDefaultOrderBy()
    {
        var rank = int.MaxValue;
        while (rank >= 0)
        {
            var c = GetColumnAtOrderRank(rank, ref rank);
            if (c == null) continue;
            SetOrderBy(c);
        }

        ApplyOrderBy();
    }

    void ApplyOrderBy()
    {
        List.ResetOrderBy();
        var c = _orderByColumn;
        while (c != null)
        {
            List.AddOrderBy(c.OrderBy, c.SortDirection);
            c = c.OrderByNext;
        }
    }

    public void Populate(object grid)
    {
        if (grid is not DataGrid dataGrid) return;

        foreach (var column in _dict.Values)
        {
            if (column.Hidden) continue;

            var header = new ColumnHeaderView
            {
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
                ApplyOrderBy();
                List.Update();
            };

            if (column.Name == "Expander")
            {
                dataGrid.RowDetailsTemplate = column.DataTemplate as IDataTemplate;
            }
            else
            {
                var c = new DataGridTemplateColumn
                {
                    Header = header,
                    CellTemplate = column.DataTemplate as IDataTemplate,
                    Width = double.IsFinite(column.Width)
                        ? new DataGridLength(column.Width)
                        : DataGridLength.Auto,
                };

                dataGrid.Columns.Add(c);
            }
        }
    }

    public object BuildTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) template = XamlTool.ContentPlaceHolder;

        var source = @$"<DataTemplate
                    xmlns=""https://github.com/avaloniaui""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    {template}
            </DataTemplate>";

        Debug.WriteLine(source);

        return AvaloniaRuntimeXamlLoader.Parse(source);
    }

    class Property(Func<T, bool> canEvaluate, Func<T, object?> getter)
    {
        public Func<T, bool> CanEvaluate { get; } = canEvaluate;
        public Func<T, object?> Getter { get; } = getter;
    }

    readonly Dictionary<string, Property> _properties = new();

    static string? PathFromExpression<TOut>(Expression<Func<T, TOut>> getter)
    {
        try
        {
            var path = TriggerPath.Factory(getter);
            return $"Model.{path}";
        }
        catch
        {
            return null;
        }
    }

    int id = 0;

    public string AddProperty<TOut>(Expression<Func<T, TOut>> getter)
    {
        var path = PathFromExpression(getter);

        if (path != null) return path;

        return AddProperty($"_{id++}", e => true, getter.CastReturn(default(object)).Compile());
    }

    public string AddProperty(Func<T, bool> condition, Func<T, object> getter)
    {
        return AddProperty($"_{id++}", condition, getter);
    }

    public string AddProperty(string name, Func<T, bool> condition, Func<T, object> getter)
    {
        _properties.Add(name, new(condition, getter));
        return name;
    }

    public void AddColumn(IColumn<T> column)
    {
        _dict.Add(column.Name, column);
    }

    void RegisterTriggers(T model, Action<string> handler)
    {
        foreach (var column in _dict.Values)
        {
            column.RegisterTriggers(model, handler);
        }
    }

    void IColumnsProvider<T>.RegisterTriggers(T model, Action<string> handler)
    {
        RegisterTriggers(model, handler);
    }
}
