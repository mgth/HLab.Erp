using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HLab.Base.Avalonia.DependencyHelpers;
using HLab.Erp.Data;

namespace HLab.Erp.Core.Avalonia.EntityLists;

using H = DependencyHelper<ColumnHeaderView>;

public class SortDirectionEventArg(object source, SortDirection sortDirection) : EventArgs
{
    public object Source { get; } = source;
    public SortDirection SortDirection { get; } = sortDirection;
}

public delegate void SortDirectionEventHandler(object sender, SortDirectionEventArg e);

/// <summary>
/// Logique d'interaction pour ColumnHeaderView.axaml
/// </summary>
public partial class ColumnHeaderView : UserControl
{
    public ColumnHeaderView()
    {
        InitializeComponent();
    }

    public static readonly global::Avalonia.StyledProperty<SortDirection> SortDirectionProperty = H.Property<SortDirection>()
        .OnChanged((e, a) => e.OnSortDirectionChange(a.NewValue.Value))
        .BindModeDefault(global::Avalonia.Data.BindingMode.TwoWay)
        .Default(SortDirection.None)
        .Register();

    public static readonly global::Avalonia.StyledProperty<int> OrderByOrderProperty = H.Property<int>()
        .BindModeDefault(global::Avalonia.Data.BindingMode.TwoWay)
        .Register();

    public event SortDirectionEventHandler? SortDirectionChanged;

    void OnSortDirectionChange(SortDirection value)
    {
        SortingIcon.Path = value switch
        {
            SortDirection.Ascending => "icons/sort/ascending",
            SortDirection.Descending => "icons/sort/descending",
            SortDirection.None => "icons/sort/none",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
        SortDirectionChanged?.Invoke(this, new SortDirectionEventArg(this, value));
    }

    public SortDirection SortDirection
    {
        get => GetValue(SortDirectionProperty);
        set => SetValue(SortDirectionProperty, value);
    }

    public int OrderByOrder
    {
        get => GetValue(OrderByOrderProperty);
        set => SetValue(OrderByOrderProperty, value);
    }

    void ButtonBase_OnClick(object? sender, RoutedEventArgs e)
    {
        SortDirection = SortDirection switch
        {
            SortDirection.None => SortDirection.Ascending,
            SortDirection.Ascending => SortDirection.Descending,
            SortDirection.Descending => SortDirection.None,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
