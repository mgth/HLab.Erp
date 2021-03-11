using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour ListFilterDateView.xaml
    /// </summary>
    public partial class FilterDateView : UserControl, IView<ViewModeDefault, DateFilter> , IFilterContentViewClass
    {
        public FilterDateView()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
        }
    }
}
