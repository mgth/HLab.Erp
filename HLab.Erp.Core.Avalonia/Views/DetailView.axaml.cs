using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Erp.Core.Avalonia.Views;

using H = DependencyHelper<DetailView>;

/// <summary>
/// Fiche de détail : bandeau (icône + titre localisé) et grille auto label/champ.
/// </summary>
public partial class DetailView : UserControl
{
    public DetailView()
    {
        InitializeComponent();
    }

    public bool EditMode
    {
        get => GetValue(EditModeProperty);
        set => SetValue(EditModeProperty, value);
    }
    public static readonly StyledProperty<bool> EditModeProperty = H.Property<bool>().Register();

    public string? IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public static readonly StyledProperty<string?> IconPathProperty =
        H.Property<string?>()
            .OnChanged((s, a) => s.HeaderIcon.Path = a.NewValue.Value)
            .Register();

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string?> TextProperty =
        H.Property<string?>()
            .OnChanged((s, a) => s.HeaderText.Id = a.NewValue.Value)
            .Register();

    [Content]
    public Controls Children => PART_Host.Children;
}
