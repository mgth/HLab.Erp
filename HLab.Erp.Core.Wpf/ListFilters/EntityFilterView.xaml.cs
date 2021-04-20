using System.Linq;
using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterEntityView.xaml
    /// </summary>
    public partial class EntityFilterView : UserControl , IView<ViewModeDefault, IEntityFilterViewModel>, IFilterContentViewClass
    {
        public EntityFilterView()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
            var content = ContentControl.FindVisualChildren<FilterView>().FirstOrDefault();
            if (content != null)
            {
                content.ToggleButton.IsChecked = false;
                content.ToggleButton.IsChecked = true;
            }
        }
    }
}
