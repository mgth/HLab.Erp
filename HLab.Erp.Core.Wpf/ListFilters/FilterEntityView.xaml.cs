using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterEntityView.xaml
    /// </summary>
    public partial class FilterEntityView : UserControl , IView<ViewModeDefault, IEntityFilterViewModel>, IFilterContentViewClass
    {
        public FilterEntityView()
        {
            InitializeComponent();
        }
    }
}
