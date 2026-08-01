using Avalonia.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.ListFilters;

/// <summary>
/// Logique d'interaction pour DateFilterView.axaml
/// </summary>
public partial class DateFilterView : UserControl,
    IView<DefaultViewMode, DateFilter>,
    IView<DefaultViewMode, DateFilterNullable>
    , IFilterContentViewClass
{
    public DateFilterView()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
        MinDatePicker.Focus();
    }
}
