using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Erp.Core.Avalonia.EntityLists;

using H = DependencyHelper<ColumnDescriptionBlock>;

/// <summary>
/// Bloc titre + description pour colonne de liste.
/// </summary>
public partial class ColumnDescriptionBlock : UserControl
{
    public ColumnDescriptionBlock()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string?> TitleProperty = H.Property<string?>()
        .BindModeDefault(BindingMode.TwoWay)
        .Register();

    public static readonly StyledProperty<string?> DescriptionProperty = H.Property<string?>().OnChanged(
        (e, a) =>
        {
            var v = a.NewValue.Value?.TrimEnd('\r', '\n', ' ');
            if (v != e.Description) e.Description = v;
        })
        .BindModeDefault(BindingMode.TwoWay)
        .Register();

    public static readonly StyledProperty<string?> IconPathProperty = H.Property<string?>()
        .BindModeDefault(BindingMode.TwoWay)
        .Register();

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }
}
