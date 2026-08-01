using Avalonia.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.ListFilters;

/// <summary>
/// Logique d'interaction pour BoolFilterView.axaml
/// </summary>
public partial class BoolFilterView : UserControl, IView<DefaultViewMode, BoolFilter>, IFilterContentViewClass
{
    public BoolFilterView()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
        CheckBox.Focus();
    }
}
