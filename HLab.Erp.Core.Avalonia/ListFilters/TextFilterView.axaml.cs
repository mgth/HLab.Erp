using Avalonia.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Avalonia.ListFilters;

/// <summary>
/// Logique d'interaction pour TextFilterView.axaml
/// </summary>
public partial class TextFilterView : UserControl, IView<DefaultViewMode, TextFilter>, IFilterContentViewClass
{
    public TextFilterView()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
        TextBox.Focus();
    }
}
