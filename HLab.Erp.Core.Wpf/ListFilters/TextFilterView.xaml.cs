using System.Windows.Controls;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    /// <summary>
    /// Logique d'interaction pour FilterTextView.xaml
    /// </summary>
    public partial class TextFilterView : UserControl, IView<ViewModeDefault, TextFilter>, IFilterContentViewClass
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
}
