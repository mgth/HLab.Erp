using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf.DependencyProperties;

namespace HLab.Erp.Core.Wpf.Views;

using H = DependencyHelper<ProgressView>;
/// <summary>
/// Logique d'interaction pour ProgressView.xaml
/// </summary>
public partial class ProgressView : UserControl//, IView<DefaultViewMode, ProgressViewModel>
{
    public ProgressView()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        H.Property<double>()
            .OnChange((e, a) =>
            {
                e.ProgressBar.Value = a.NewValue;
                e.Label.Content = a.NewValue;
            })
            .Register();

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

}