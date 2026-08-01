using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.ListFilters;

/// <summary>
/// Logique d'interaction pour EntityFilterView.axaml
/// </summary>
public partial class EntityFilterView : UserControl, IView<DefaultViewMode, IEntityFilterViewModel>, IFilterContentViewClass
{
    public EntityFilterView()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
        var content = this.GetVisualDescendants().OfType<FilterView>().FirstOrDefault();
        if (content != null)
        {
            content.ToggleButton.IsChecked = false;
            content.ToggleButton.IsChecked = true;
        }
    }
}
