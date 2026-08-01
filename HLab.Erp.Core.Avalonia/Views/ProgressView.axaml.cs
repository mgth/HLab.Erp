using Avalonia;
using Avalonia.Controls;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Erp.Core.Avalonia.Views;

using H = DependencyHelper<ProgressView>;

/// <summary>
/// Logique d'interaction pour ProgressView.axaml
/// </summary>
public partial class ProgressView : UserControl
{
    public ProgressView()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<double> ValueProperty =
        H.Property<double>()
            .OnChanged((e, a) =>
            {
                e.ProgressBar.Value = a.NewValue.Value;
                e.Label.Text = $"{a.NewValue.Value:P0}";
            })
            .Register();

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
