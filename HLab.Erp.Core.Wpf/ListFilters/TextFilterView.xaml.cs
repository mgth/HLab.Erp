using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterTextView.xaml
    /// </summary>
    public partial class FilterTextView : UserControl, IView<ViewModeDefault, TextFilter>, IFilterContentViewClass
    {
        public FilterTextView()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
            TextBox.Focus();
        }
    }
}
