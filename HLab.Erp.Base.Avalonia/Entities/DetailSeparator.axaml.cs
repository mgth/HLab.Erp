using Avalonia;
using Avalonia.Controls;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Erp.Base.Avalonia.Entities;

using H = DependencyHelper<DetailSeparator>;

/// <summary>
/// Séparateur de fiche : icône + libellé localisé.
/// </summary>
public partial class DetailSeparator : UserControl
{
    public DetailSeparator()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string?> IconPathProperty =
        H.Property<string?>()
            .OnChanged((s, e) => s.IconView.Path = e.NewValue.Value)
            .Register();

    public string? IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public static readonly StyledProperty<string?> TextProperty =
        H.Property<string?>()
            .OnChanged((s, e) => s.Localize.Id = e.NewValue.Value)
            .Register();

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
