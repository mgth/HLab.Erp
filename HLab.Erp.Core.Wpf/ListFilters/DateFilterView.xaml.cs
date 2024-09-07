using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ListFilters
{ 
    /// <summary>
    /// Logique d'interaction pour ListFilterDateView.xaml
    /// </summary>
public partial class DateFilterView : UserControl, 
        IView<DefaultViewMode, DateFilter> ,
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
}
