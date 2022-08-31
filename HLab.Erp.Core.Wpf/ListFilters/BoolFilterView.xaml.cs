using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterTextView.xaml
    /// </summary>
    public partial class BoolFilterView : UserControl, IView<ViewModeDefault, BoolFilter>, IFilterContentViewClass
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
}
