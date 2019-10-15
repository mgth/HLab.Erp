using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl, IView<ViewModeDefault, IFilterViewModel>
    {
        public FilterView()
        {
            InitializeComponent();
        }
    }
}
