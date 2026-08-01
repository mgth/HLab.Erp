using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.ListFilters;

/// <summary>
/// Logique d'interaction pour FilterView.axaml
/// </summary>
public partial class FilterView : UserControl, IView<DefaultViewMode, IFilter>
{
    public FilterView()
    {
        InitializeComponent();

        ToggleButton.IsCheckedChanged += ToggleButton_Checked;
    }

    void ToggleButton_Checked(object? sender, RoutedEventArgs e)
    {
        if (ToggleButton.IsChecked != true) return;

        var content = this.GetVisualDescendants().OfType<IFilterContentViewClass>();
        foreach (var control in content)
        {
            control.SetFocus();
        }
    }
}
